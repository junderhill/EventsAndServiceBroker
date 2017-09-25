using System.Diagnostics;
using Quartz;

namespace MSMQTest
{
    public class ScheduledJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("Execute Slow Scheduled Job");
            
            EventLoop.Instance.Enqueue(new SlowCalculateBigPrimeEvent());
        }
    }
}