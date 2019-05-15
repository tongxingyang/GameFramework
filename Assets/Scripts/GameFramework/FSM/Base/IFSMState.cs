namespace GameFramework.FSM
{
    public interface IFSMState
    {
        void OnInit(IFSMStateMachine fsm);
        void OnEnter(IFSMStateMachine fsm);
        void OnUpdate(IFSMStateMachine fsm, float elapseSeconds, float realElapseSeconds);
        void OnExit(IFSMStateMachine fsm,bool isShutdown);
        void OnDestroy(IFSMStateMachine fsm);
        void SubscribeEvent(int eventId, FSMManager.FsmEventHandler eventHandler);
        void UnsubscribeEvent(int eventId, FSMManager.FsmEventHandler eventHandler);
        void OnEvent(IFSMStateMachine fsm, object sender, int eventId, object userData);
    }
}