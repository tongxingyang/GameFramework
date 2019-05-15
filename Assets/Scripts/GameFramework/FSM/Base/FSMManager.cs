using System;
using System.Collections.Generic;
using GameFramework.Debug;

namespace GameFramework.FSM
{
    public class FSMManager : IFSMManager
    {
        public delegate void FsmEventHandler(IFSMManager fsm, object sender, object userData);
        private List<string> destoryFSMList;
        private readonly Dictionary<string, IFSMStateMachine> fsmStateMachines;
        public int Count => fsmStateMachines.Count;

        public FSMManager()
        {
            destoryFSMList = new List<string>();
            fsmStateMachines = new Dictionary<string, IFSMStateMachine>();
        }
        
        public bool HasFSMStateMachine<T>() where T : IFSMStateMachine
        {
            if (fsmStateMachines.ContainsKey(typeof(T).FullName))
            {
                return true;
            }
            return false;
        }

        public IFSMStateMachine GetFSMStateMachine<T>() where T : IFSMStateMachine
        {
            IFSMStateMachine stateMachine = null;
            if (fsmStateMachines.TryGetValue(typeof(T).FullName, out stateMachine))
            {
                return stateMachine;
            }
            return null;
        }

        public IFSMStateMachine[] GetAllFSMStateMachines()
        {
            int index = 0;
            IFSMStateMachine[] results = new IFSMStateMachine[fsmStateMachines.Count];
            foreach (KeyValuePair<string, IFSMStateMachine> fsm in fsmStateMachines)
            {
                results[index++] = fsm.Value;
            }
            return results;
        }

        public IFSMStateMachine CreateFSMStateMachine<T>(IFSMState[] states) where T : IFSMStateMachine
        {
            if (HasFSMStateMachine<T>())
            {
                Debuger.LogError("already add fsmmachine name "+typeof(T).FullName);
                return null;
            }
            IFSMStateMachine machine = new FSMStateMachine(typeof(T).FullName,states);
            fsmStateMachines.Add(typeof(T).FullName,machine);
            return machine;
        }
        
        public IFSMStateMachine AddFSMStateMachine<T>(IFSMState state) where T : IFSMStateMachine
        {
            IFSMStateMachine machine = GetFSMStateMachine<T>();
            if (machine != null)
            {
                machine.AddState(state);
            }
            return null;
        }


        public void DestroyFSMStateMachine<T>() where T : IFSMStateMachine
        {
            if (HasFSMStateMachine<T>())
            {
                destoryFSMList.Add(typeof(T).FullName);
            }
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var destoryItem in destoryFSMList)
            {
                fsmStateMachines.Remove(destoryItem);
            }
            destoryFSMList.Clear();

            foreach (var machine in fsmStateMachines)
            {
                machine.Value.OnUpdate(elapseSeconds, realElapseSeconds);
            }
        }

        public void Shutdown()
        {
            foreach (KeyValuePair<string, IFSMStateMachine> machine in fsmStateMachines)
            {
                machine.Value.Shutdown();
            }
            fsmStateMachines.Clear();
            destoryFSMList.Clear();
        }
    }
}