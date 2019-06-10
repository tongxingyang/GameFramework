using System;
using System.Collections.Generic;
using GameFramework.Debug;

namespace GameFramework.FSM
{
    public class FSMManager : IFSMManager
    {
        public delegate void FsmEventHandler(IFSMManager fsm, object sender, object userData);
        
        private readonly Dictionary<string, IFSMStateMachine> fsmStateMachines;
        private List<string> destroyFSMList;
        public int Count => fsmStateMachines?.Count ?? 0;

        public FSMManager()
        {
            destroyFSMList = new List<string>();
            fsmStateMachines = new Dictionary<string, IFSMStateMachine>();
        }
        
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var destroyItem in destroyFSMList)
            {
                fsmStateMachines.Remove(destroyItem);
            }
            destroyFSMList.Clear();

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
            destroyFSMList.Clear();
            destroyFSMList = null;
        }
        
        public bool HasFSMStateMachine<T>() where T : IFSMStateMachine
        {
            return HasFSMStateMachine<T>(string.Empty);
        }

        public bool HasFSMStateMachine<T>(string name) where T : IFSMStateMachine
        {
            if (fsmStateMachines.ContainsKey(Utility.StringUtility.GetFullName<T>(name)))
            {
                return true;
            }
            return false;
        }

        public IFSMStateMachine GetFSMStateMachine<T>() where T : IFSMStateMachine
        {
            return GetFSMStateMachine<T>(string.Empty);
        }

        public IFSMStateMachine GetFSMStateMachine<T>(string name) where T : IFSMStateMachine
        {
            if (fsmStateMachines.TryGetValue(Utility.StringUtility.GetFullName<T>(name), out var fsmStateMachine))
            {
                return fsmStateMachine;
            }
            return null;
        }

        public IFSMStateMachine CreateFSMStateMachine<T>(IFSMState[] states) where T : IFSMStateMachine
        {
            return CreateFSMStateMachine<T>(String.Empty, states);
        }

        public IFSMStateMachine CreateFSMStateMachine<T>(string name, IFSMState[] states) where T : IFSMStateMachine
        {
            if (HasFSMStateMachine<T>(name))
            {
                return null;
            }
            IFSMStateMachine machine = new FSMStateMachine(Utility.StringUtility.GetFullName<T>(name),states);
            fsmStateMachines.Add(Utility.StringUtility.GetFullName<T>(name),machine);
            return machine;
        }

        public void DestroyFSMStateMachine<T>() where T : IFSMStateMachine
        {
            DestroyFSMStateMachine<T>(String.Empty);
        }

        public void DestroyFSMStateMachine<T>(string name) where T : IFSMStateMachine
        {
            if (HasFSMStateMachine<T>(Utility.StringUtility.GetFullName<T>(name)))
            {
                destroyFSMList.Add(Utility.StringUtility.GetFullName<T>(name));
            }
        }

        public IFSMStateMachine AddFSMStateMachine<T>(IFSMState state) where T : IFSMStateMachine
        {
            return AddFSMStateMachine<T>(String.Empty, state);
        }

        public IFSMStateMachine AddFSMStateMachine<T>(string name, IFSMState state) where T : IFSMStateMachine
        {
            if (HasFSMStateMachine<T>(Utility.StringUtility.GetFullName<T>(name)))
            {
                IFSMStateMachine machine = GetFSMStateMachine<T>(Utility.StringUtility.GetFullName<T>(name));
                if (machine != null)
                {
                    machine.AddState(state);
                    return machine;
                }
                return null;
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
    }
} 