namespace MSMQTest
{


    public class TestMessageEvent : IEvent
    {
        public string Message { get; set; }

            public TestMessageEvent(string message)
            {
                Message = message;
            }

        }
}