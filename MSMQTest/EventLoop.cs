using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Quartz.Xml.JobSchedulingData20;

namespace MSMQTest
{
    public class EventLoop
    {
        private bool _running = false;
        private ConcurrentQueue<IEvent> _events;
        private ManualResetEventSlim Wait;
        
        private static EventLoop _eventLoop;
        public static EventLoop Instance
        {
            get
            {
                if (_eventLoop == null)
                {
                       _eventLoop = new EventLoop(); 
                }
                return _eventLoop;
            }
        }
        
        public EventLoop()
        {
            _events = new ConcurrentQueue<IEvent>();
            Wait = new ManualResetEventSlim(false);
        }

        public int QueuedJobs => this._events.Count;

        public void Start()
        {
            Task.Factory.StartNew(Loop);
            _running = true;
        }

        public void Stop()
        {
            _running = false;
            Wait.Set();
        }

        private void Loop()
        {
            while (_running)
            {
                IEvent e;
                if (_events.TryDequeue(out e))
                {
                    Type handlerType = typeof(IEventHandler<>).MakeGenericType(e.GetType());
                    var handlers = Program.Container.GetAllInstances(handlerType);
                    foreach (dynamic h in handlers)
                    {
                        h.Execute((dynamic) e);
                    }
                }
                else
                {
                    Wait.Reset();
                    Wait.Wait();
                }
            }
        }

        public void Enqueue<T>(T e) where T : IEvent
        {
            _events.Enqueue(e);
            Wait.Set();
        }
    }
}