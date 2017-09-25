using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace MSMQTest
{
    public class TestService
    {
        private bool _montioringQueue;
        private IScheduler scheduler;

        public void Start()
        {
            EventLoop.Instance.Start();

            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            SetupSlowRepetativeJob(EventLoop.Instance, scheduler);

            _montioringQueue = true;
            Task.Factory.StartNew(MonitorQueue);

        }

        public void Stop()
        {
            EventLoop.Instance.Stop();
            _montioringQueue = false;
            scheduler.Shutdown();
        }
        
        private static void SetupSlowRepetativeJob(EventLoop eventLoop, IScheduler scheduler)
        {
            Debug.WriteLine("Setup Slow Repetative Job");
            
            IJobDetail job = JobBuilder.Create<ScheduledJob>()
                .WithIdentity("job1", "group1")
                .Build();
            
            IJobDetail queuryEventLoopJob = JobBuilder.Create<QueryEventLoopJob>()
                .WithIdentity("eventLoopQuery", "group1")
                .Build();
            ITrigger queryTrigger = TriggerBuilder.Create()
                .WithIdentity("eventLoopQueryTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(3).RepeatForever())
                .Build();
            scheduler.ScheduleJob(queuryEventLoopJob, queryTrigger);

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever())
                .Build();

            
            scheduler.ScheduleJob(job, trigger);
        }

        private const string Connectionstring = "Data Source=BWS-SQL;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Database=Dev_ServiceBroker;MultipleActiveResultSets=True";
        private void MonitorQueue()
        {
            string commandText = "WAITFOR( RECEIVE CONVERT(NVARCHAR(MAX), message_body) AS Message FROM SBReceiveQueue);";

            while (_montioringQueue)
            {
                using (SqlConnection connection = new SqlConnection(Connectionstring))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(commandText, connection))
                    {
                        Debug.WriteLine("Waiting for message");
                        command.CommandTimeout = 0;
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            EventLoop.Instance.Enqueue(new TestMessageEvent(reader.GetString(0)));
                            EventLoop.Instance.Enqueue(new SlowCalculateBigPrimeEvent());
                        }
                    }
                }
            }
        }
    }
}