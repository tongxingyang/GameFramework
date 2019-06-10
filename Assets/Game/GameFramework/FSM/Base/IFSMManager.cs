namespace GameFramework.FSM
{
    public interface IFSMManager
    {
        int Count{get;}
        bool HasFSMStateMachine<T>() where T : IFSMStateMachine;
        bool HasFSMStateMachine<T>(string name) where T : IFSMStateMachine;
        IFSMStateMachine GetFSMStateMachine<T>() where T : IFSMStateMachine;
        IFSMStateMachine GetFSMStateMachine<T>(string name) where T : IFSMStateMachine;
        IFSMStateMachine CreateFSMStateMachine<T>(IFSMState[] states) where T : IFSMStateMachine;
        IFSMStateMachine CreateFSMStateMachine<T>(string name,IFSMState[] states) where T : IFSMStateMachine;
        void DestroyFSMStateMachine<T>() where T : IFSMStateMachine;
        void DestroyFSMStateMachine<T>(string name) where T : IFSMStateMachine;
        IFSMStateMachine AddFSMStateMachine<T>(IFSMState state) where T : IFSMStateMachine;
        IFSMStateMachine AddFSMStateMachine<T>(string name,IFSMState state) where T : IFSMStateMachine;
        IFSMStateMachine[] GetAllFSMStateMachines();
    }
}