namespace GameFramework.Pool.TaskPool
{
    public interface ITask
    {
        int SerialId { get; }
        bool Done { get; }
        int Priority { get; }
        void Clear();
    }
}