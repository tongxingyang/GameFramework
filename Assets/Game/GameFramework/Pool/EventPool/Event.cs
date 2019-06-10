namespace GameFramework.Pool.EventPool
{
    public sealed class Event<T> where T: GameEventArgs
    {
        private readonly object sender;
        private readonly T eventArgs;
        
        public object Sender => sender;
        public T EventArgs => eventArgs;
        
        public Event(object sender, T eventArgs)
        {
            this.sender = sender;
            this.eventArgs = eventArgs;
        }
    }
}