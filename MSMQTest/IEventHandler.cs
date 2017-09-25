namespace MSMQTest
{
    public interface IEventHandler<IEvent>
    {
        void Execute(IEvent e);
    }
}