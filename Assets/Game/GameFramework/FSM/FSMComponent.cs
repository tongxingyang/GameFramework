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

        public bool HasFSMStateMachine<T>(string name) where T : IFSMStateMachine
        {
            return fsmManager.HasFSMStateMachine<T>(name);
        }

        public IFSMStateMachine GetFSMStateMachine<T>() where T : IFSMStateMachine
        {
            return fsmManager.GetFSMStateMachine<T>();
        }

        public IFSMStateMachine GetFSMStateMachine<T>(string name) where T : IFSMStateMachine
        {
            return fsmManager.GetFSMStateMachine<T>(name);
        }

        public IFSMStateMachine CreateFSMStateMachine<T>(IFSMState[] states) where T : IFSMStateMachine
        {
            return fsmManager.CreateFSMStateMachine<T>(states);
        }

        public IFSMStateMachine CreateFSMStateMachine<T>(string name, IFSMState[] states) where T : IFSMStateMachine
        {
            return fsmManager.CreateFSMStateMachine<T>(name,states);
        }

        public void DestroyFSMStateMachine<T>() where T : IFSMStateMachine
        {
            fsmManager.DestroyFSMStateMachine<T>();
        }

        public void DestroyFSMStateMachine<T>(string name) where T : IFSMStateMachine
        {
            fsmManager.DestroyFSMStateMachine<T>(name);
        }

        public IFSMStateMachine AddFSMStateMachine<T>(IFSMState state) where T : IFSMStateMachine
        {
            return fsmManager.AddFSMStateMachine<T>(state);
        }

        public IFSMStateMachine AddFSMStateMachine<T>(string name, IFSMState state) where T : IFSMStateMachine
        {
            return fsmManager.AddFSMStateMachine<T>(name,state);
        }

        public IFSMStateMachine[] GetAllFSMStateMachines()
        {
            return fsmManager.GetAllFSMStateMachines();
        }
    }
}