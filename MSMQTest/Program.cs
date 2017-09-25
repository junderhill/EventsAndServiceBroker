using System;
using System.Collections.Generic;
using Quartz.Xml.JobSchedulingData20;
using SimpleInjector;
using Topshelf;

namespace MSMQTest
{
    internal class Program
    {

        public static void Main(string[] args)
        {
            ConfigureIOCContainer();
            
            HostFactory.Run(x =>
            {
                x.Service<TestService>(s =>
                {
                    s.ConstructUsing(t => new TestService());
                    s.WhenStarted(t => t.Start());
                    s.WhenStopped(t => t.Stop());
                });

                x.RunAsLocalService();
            });
        }

        private static void ConfigureIOCContainer()
        {
            Container = new Container();
            var assemblies = new[] {typeof(IEventHandler<>).Assembly};
            Container.RegisterCollection(typeof(IEventHandler<>), assemblies);
            Container.Verify();
        }

        public static  Container Container { get; private set; }
    }
}