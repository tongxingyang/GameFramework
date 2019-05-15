namespace GameFramework.Pool.EventPool
{
    public sealed class Event<T> where T: GameEventArgs
    {
        private readonly object sender;
        private readonly T eventArgs;
        public Event(object s, T e)
        {
            sender = s;
            eventArgs = e;
        }

        public object Sender => sender;

        public T EventArgs => eventArgs;
    }
}