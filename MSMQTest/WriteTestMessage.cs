using System;

namespace MSMQTest
{
    public class WriteTestMessage : IEventHandler<TestMessageEvent>
    {
        public void Execute(TestMessageEvent e)
        {
            Console.WriteLine(e.Message);
        }
    }
}