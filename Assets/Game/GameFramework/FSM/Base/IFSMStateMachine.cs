using System;
using GameFramework.Pool.EventPool;

namespace GameFramework.FSM
{
    public interface IFSMStateMachine
    {
        string Name{get;}
        bool isRunning { get; }
        IFSMState CurrentState{get;}
        IFSMState PreviousState{get;}
        float CurrentStateTime{get;}
        int FSMStateCount{get;}
        void Start<T>() where T :class, IFSMState;
        bool HasState<T>() where T :class, IFSMState;
        IFSMState[] GetAllStates();
        void FireEvent(object sender, GameEventArgs e);
        void OnUpdate(float elapseSeconds, float realElapseSeconds);
        T GetState<T>() where T : class ,IFSMState ;
        void AddState(IFSMState state);
        void ChangeState<T>() where T :class,  IFSMState;
        void Shutdown();
    }
    /*
     

     */
}