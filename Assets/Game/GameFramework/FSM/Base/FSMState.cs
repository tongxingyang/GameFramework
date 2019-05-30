namespace GameFramework.FSM
{
    public abstract class FSMState
    {
        public abstract void OnInit(IFSMStateMachine fsm);
        public abstract void OnEnter(IFSMStateMachine fsm);
        public abstract void OnUpdate(IFSMStateMachine fsm, float elapseSeconds, float realElapseSeconds);
        public abstract void OnExit(IFSMStateMachine fsm, bool isShutdown);
        public abstract void OnDestroy(IFSMStateMachine fsm);
        public abstract void SubscribeEvent(int eventId, FSMManager.FsmEventHandler eventHandler);
        public abstract void UnsubscribeEvent(int eventId, FSMManager.FsmEventHandler eventHandler);
        public abstract void OnEvent(IFSMStateMachine fsm, object sender, int eventId, object userData);
    }
}