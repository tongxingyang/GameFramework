using System;
using System.Collections.Generic;
using GameFramework.Debug;
using GameFramework.Pool.EventPool;

namespace GameFramework.FSM
{
    public class FSMStateMachine : IFSMStateMachine
    {
        private string name;
        public string Name => name;
        public bool isRunning => currentState != null;
        private IFSMState previousState;
        public IFSMState PreviousState => previousState;
        private IFSMState currentState;
        public IFSMState CurrentState => currentState;
        private float currentStateTime;
        public float CurrentStateTime => currentStateTime;
        public int FSMStateCount => statesDic?.Count ?? 0;
        private readonly Dictionary<string, IFSMState> statesDic;

        public FSMStateMachine(string strName, params IFSMState[] states)
        {
            name = strName ?? string.Empty;
            statesDic = new Dictionary<string, IFSMState>();
            foreach (var state in states)
            {
                if (state == null)
                {
                    Debuger.LogError("FSM state is invalid.");
                }
                string strStateName = state.GetType().FullName;
                if (statesDic.ContainsKey(strStateName))
                {
                    Debuger.LogError(Utility.StringUtility.Format("FSM '{0}' state '{1}' is already exist.", strName,
                        strStateName));
                }
                state.OnInit(this);
                statesDic.Add(strStateName, state);
            }
            currentStateTime = 0f;
            currentState = null;
            previousState = null;
        }

        public void Start<T>() where T : class, IFSMState
        {
            if (isRunning)
            {
                Debuger.LogError("error fsm is already running...");
            }
            IFSMState state = GetState<T>();
            currentStateTime = 0f;
            if (state == null)
            {
                Debuger.LogError("fsm is not content state name :" + typeof(T).FullName);
                return;
            }
            currentState = state;
            currentState.OnEnter(this);
        }

        public bool HasState<T>() where T :class, IFSMState
        {
            return statesDic.ContainsKey(typeof(T).FullName);
        }

        public IFSMState[] GetAllStates()
        {
            int index = 0;
            IFSMState[] results = new IFSMState[statesDic.Count];
            foreach (KeyValuePair<string, IFSMState> state in statesDic)
            {
                results[index++] = state.Value;
            }
            return results;
        }

        public void FireEvent(object sender, GameEventArgs e)
        {
            if (currentState == null)
            {
                Debuger.LogError("Current state is invalid.");
            }
            currentState.OnEvent(sender,e);
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (currentState == null)
            {
                return;
            }
            currentStateTime += elapseSeconds;
            currentState.OnUpdate(this, elapseSeconds, realElapseSeconds);
        }

        public T GetState<T>() where T :class, IFSMState
        {
            if (statesDic.TryGetValue(typeof(T).FullName, out var state))
            {
                return (T) state;
            }
            return null;
        }

        public void AddState(IFSMState state)
        {
            state.OnInit(this);
            statesDic.Add(state.GetType().FullName, state);
        }

        public void ChangeState<T>() where T :class, IFSMState
        {
            if (currentState == null)
            {
                Debuger.LogError("Current state is invalid.");
                return;
            }
            IFSMState state = GetState<T>();
            if (state == null)
            {
                Debuger.LogError(Utility.StringUtility.Format("FSM '{0}' can not change state to '{1}' which is not exist.", Utility.StringUtility.GetFullName<T>(Name), typeof(T).FullName));
                return;
            }
            currentState.OnExit(this, false);
            previousState = currentState;
            currentStateTime = 0f;
            currentState = state;
            currentState.OnEnter(this);
        }

        public void Shutdown()
        {
            if (currentState != null)
            {
                currentState.OnExit(this, true);
                currentState = null;
                previousState = null;
                currentStateTime = 0f;
            }
            foreach (KeyValuePair<string, IFSMState> state in statesDic)
            {
                state.Value.OnDestroy(this);
            }
            statesDic.Clear();
        }
    }
}