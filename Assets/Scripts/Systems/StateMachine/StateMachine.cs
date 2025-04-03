using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using EditorAttributes;
using AYellowpaper.SerializedCollections;
using UltEvents;

namespace ObjectBasedStateMachine.UnLayered
{
    /// <summary>
    /// The Overarching controller of a State Machine object. <br />
    /// Override this class to create more specific StateMachines with more easily accessible components. <br />
    /// Although most of the time it's probably not necessary.
    /// </summary>
    [DefaultExecutionOrder(-50)]
    public class StateMachine : MonoBehaviour
    {

        #region Config

        [Button("Add New State")]
        protected void AddState()
        {
            var NSGO = new GameObject("NewState");
            NSGO.transform.parent = stateHolder.transform;
            NSGO.AddComponent<State>();
        }

        #endregion

        #region Data

        public Transform stateHolder { get; private set; }
        public State currentState { get; private set; }
        public State[] children { get; protected set; }
        public int childCount { get; protected set; }

        public System.Action waitforMachineInit;


        #endregion



        #region Real Unity Messages

        protected virtual void Awake() => this.Initialize();

        private void Reset()
        {
            Transform tryRoot = transform.Find("States");
            GameObject root = tryRoot ? tryRoot.gameObject : new GameObject("States");
            root.transform.parent = transform;
            stateHolder = root.transform;
        }

        protected virtual void Update()
        {
            currentState.DoUpdate();

            if (signalQueueDecay.running) signalQueueDecay.Tick(() =>
            {
                if (signalQueue.Count > 0) signalQueue.Dequeue();
                if (signalQueue.Count > 0) signalQueueDecay.Begin();
            });
        }

        protected virtual void FixedUpdate() => currentState.DoFixedUpdate();


        #endregion






        private void Initialize()
        {
            if (stateHolder == null)
            {
                Transform tryRoot = transform.Find("States");
                stateHolder = tryRoot != null ? tryRoot : throw new System.Exception("State Root Missing");
            }
            this.OnInitialize();

            if (stateHolder.childCount == 0) throw new System.Exception("Stateless State Machines are not supported. If you need to use StateBehaviors on something with only one state, create a dummy state.");
            else
            {
                children = new State[stateHolder.childCount];
                for (int i = 0; i < children.Length; i++)
                {
                    children[i] = stateHolder.GetChild(i).GetComponent<State>();
                    children[i].Initialize(this);
                }
            }

            currentState = children[0].EnterState(null);

            waitforMachineInit?.Invoke();
        }

        protected virtual void OnInitialize() { }

        public virtual void TransitionState(State nextState) => TransitionState(nextState, currentState);
        public virtual void TransitionState(State nextState, State prevState)
        {
            // Pre Checks
            if (
                nextState == null ||
                nextState.locked ||
                nextState == currentState ||
                nextState == prevState ||
                prevState == null ||
                prevState == this ||
                !prevState.enabled
               ) return;


            prevState.ExitState(nextState);
            nextState.EnterState(prevState);
        }

        #region Signals

        [HideInEditMode, DisableInPlayMode] public bool signalReady = true;
        public Queue<string> signalQueue = new();
        public Timer.OneTime signalQueueDecay = new(1f);
        public SerializedDictionary<string, UltEvent> globalSignals;

        public bool SendSignal(string name, bool addToQueue = true, bool overrideReady = false)
        {
            if ((signalReady || overrideReady) && EnactSignal(name)) return true;
            else if (addToQueue)
            {
                signalQueue.Enqueue(name);
                if (signalQueueDecay.length > 0) signalQueueDecay.Begin();
            }
            return false;
        }
        public bool SendSignalBasic(string name) => SendSignal(name);

        public void ReadySignal()
        {
            signalReady = true;
            while (signalQueue.Count > 0)
                if (EnactSignal(signalQueue.Dequeue()))
                    break;
        }

        private bool EnactSignal(string name)
        {
            if (currentState.signals.TryGetValue(name, out UltEvent resultEvent) || globalSignals.TryGetValue(name, out resultEvent))
            {
                resultEvent?.Invoke();
                return true;
            }
            return false;
        }

        public void FinishSignal() => SendSignal("Finish", addToQueue: false, overrideReady: true);

        #endregion Signals

    }
}

