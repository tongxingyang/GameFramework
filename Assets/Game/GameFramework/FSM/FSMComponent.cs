using GameFramework.Base;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.FSM
{
    [DisallowMultipleComponent]
    public class FSMComponent : GameFrameworkComponent
    {
        public override int Priority => SingletonMono<GameFramework>.GetInstance().FsmPriority;

        public int FSMStateMachineCount => fsmManager.Count;
        
        private FSMManager fsmManager;
        public override void OnAwake()
        {
            base.OnAwake();
            fsmManager = new FSMManager();
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            fsmManager.OnUpdate(elapseSeconds,realElapseSeconds);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            fsmManager.Shutdown();
        }

        public bool HasFSMStateMachine<T>() where T : IFSMStateMachine
        {
            return fsmManager.HasFSMStateMachine<T>();
        }

        public IFSMStateMachine GetFSMStateMachine<T>() where T : IFSMStateMachine
        {
            return fsmManager.GetFSMStateMachine<T>();
        }

        public IFSMStateMachine[] GetAllFSMStateMachines()
        {
            return fsmManager.GetAllFSMStateMachines();
        }

        public IFSMStateMachine CreateFSMStateMachine<T>(IFSMState[] states) where T : IFSMStateMachine
        {
            return fsmManager.CreateFSMStateMachine<T>(states);
        }

        public void DestroyFSMStateMachine<T>() where T : IFSMStateMachine
        {
            fsmManager.DestroyFSMStateMachine<T>();
        }

        public IFSMStateMachine AddFSMStateMachine<T>(IFSMState state) where T : IFSMStateMachine
        {
            return fsmManager.AddFSMStateMachine<T>(state);
        }
    }
}