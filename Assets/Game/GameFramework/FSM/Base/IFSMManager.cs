namespace GameFramework.FSM
{
    public interface IFSMManager
    {
        int Count{get;}
        bool HasFSMStateMachine<T>() where T : IFSMStateMachine;
        IFSMStateMachine GetFSMStateMachine<T>() where T : IFSMStateMachine;
        IFSMStateMachine[] GetAllFSMStateMachines();
        IFSMStateMachine CreateFSMStateMachine<T>(IFSMState[] states) where T : IFSMStateMachine;
        void DestroyFSMStateMachine<T>() where T : IFSMStateMachine;
        IFSMStateMachine AddFSMStateMachine<T>(IFSMState state) where T : IFSMStateMachine;
    }
}