namespace GameFramework.Pool.TaskPool
{
    public interface ITaskAgent<T> where T : ITask 
    {
        T Task { get; }
        void Initialize();
        void OnStart(T task);
        void OnUpdate(float elapseSeconds, float realElapseSeconds);
        void OnReset();
        void ShotDown();
    }
}