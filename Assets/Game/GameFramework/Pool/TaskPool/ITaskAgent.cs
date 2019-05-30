namespace GameFramework.Pool.TaskPool
{
    public interface ITaskAgent<T> where T : ITask 
    {
        T Task { get; }
        void Initialize();
        void OnUpdate(float elapseSeconds, float realElapseSeconds);
        void ShotDown();
        void OnStart(T task);
        void OnReset();
    }
}