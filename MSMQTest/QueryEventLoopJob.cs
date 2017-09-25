using System;
using Quartz;

namespace MSMQTest
{
    public  class QueryEventLoopJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"EVENT LOOP JOBS QUEUED: {EventLoop.Instance.QueuedJobs}");
        }
    }
}