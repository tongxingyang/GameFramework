using System;
using System.Collections.Generic;
using GameFramework.Debug;

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
        public int FSMStateCount => States.Count;
        private readonly Dictionary<string, IFSMState> States;

        public FSMStateMachine(string strName, params IFSMState[] states)
        {
            name = strName ?? string.Empty;
            States = new Dictionary<string, IFSMState>();
            foreach (var state in states)
            {
                if (states == null)
                {
                    Debuger.LogError("FSM states is invalid.");
                }
                string strStateName = state.GetType().FullName;
                if (States.ContainsKey(strStateName))
                {
                    Debuger.LogError(Utility.StringUtility.Format("FSM '{0}' state '{1}' is already exist.", strName,
                        strStateName));
                }
                state.OnInit(this);
                States.Add(strStateName, state);
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
            if (state == null)
            {
                Debuger.LogError("fsm is not content state name :" + typeof(T).FullName);
            }
            currentStateTime = 0f;
            currentState = state;
            currentState.OnEnter(this);
        }

        public bool HasState<T>() where T :class, IFSMState
        {
            return States.ContainsKey(typeof(T).FullName);
        }

        public IFSMState[] GetAllStates()
        {
            int index = 0;
            IFSMState[] results = new IFSMState[States.Count];
            foreach (KeyValuePair<string, IFSMState> state in States)
            {
                results[index++] = state.Value;
            }
            return results;
        }

        public void FireEvent(object sender, int eventId, object userData)
        {
            if (currentState == null)
            {
                Debuger.LogError("Current state is invalid.");
            }
            currentState.OnEvent(this, sender, eventId, userData);
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
            IFSMState state = null;
            if (States.TryGetValue(typeof(T).FullName, out state))
            {
                return (T) state;
            }
            return null;
        }

        public void AddState(IFSMState state)
        {
            state.OnInit(this);
            States.Add(state.GetType().FullName, state);
        }

        public void ChangeState<T>() where T :class, IFSMState
        {
            if (currentState == null)
            {
                Debuger.LogError("Current state is invalid.");
            }
            IFSMState state = GetState<T>();
            if (state == null)
            {
                Debuger.LogError(Utility.StringUtility.Format("FSM '{0}' can not change state to '{1}' which is not exist.", Utility.StringUtility.GetFullName<T>(Name), typeof(T).FullName));
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
            foreach (KeyValuePair<string, IFSMState> state in States)
            {
                state.Value.OnDestroy(this);
            }
            States.Clear();
        }
    }
}