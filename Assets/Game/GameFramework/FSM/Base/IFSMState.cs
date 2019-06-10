using System;
using GameFramework.Pool.EventPool;

namespace GameFramework.FSM
{
    public interface IFSMState
    {
        void OnInit(IFSMStateMachine fsm);
        void OnEnter(IFSMStateMachine fsm);
        void OnUpdate(IFSMStateMachine fsm, float elapseSeconds, float realElapseSeconds);
        void OnExit(IFSMStateMachine fsm,bool isShutdown);
        void OnDestroy(IFSMStateMachine fsm);
        void SubscribeEvent(enEventID id, EventHandler<GameEventArgs> handler);
        void UnsubscribeEvent(enEventID id, EventHandler<GameEventArgs> handler);
        void OnEvent(object sender, GameEventArgs e);
    }
}