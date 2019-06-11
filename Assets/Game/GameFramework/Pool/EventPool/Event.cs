namespace GameFramework.Pool.EventPool
{
    public sealed class Event<T> where T: GameEventArgs
    {
        private object sender;
        private T eventArgs;
        private bool isUsed;

        public object Sender
        {
            get => sender;
            set => sender = value;
        }

        public T EventArgs
        {
            get => eventArgs;
            set => eventArgs = value;
        }

        public bool IsUsed
        {
            get => isUsed;
            set => isUsed = value;
        }
        
        public void Reset()
        {
            this.isUsed = false;
            this.sender = null;
            this.eventArgs = null;
        }
    }
}