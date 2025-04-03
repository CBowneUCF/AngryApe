using UnityEngine;
using EditorAttributes;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UltEvents;

namespace ObjectBasedStateMachine.UnLayered
{
    //NOTE TO SELF: State Machine inheriting from State and thus gaining all of its State-specific data pieces is a pain in the ass. Look into fixing later with a shared Ancestor.
    //NOTE TO SELF 2: Multi-Layered State Machines are also a Pain in the ass. Consider Version 3 where there's no layering but each State can be in a group.
    //NOTE TO SELF 3: Maybe have "Previous State" as a saved reference in Machin in V3?

    /// <summary>
    /// The class for an individual State in the State Machine. I wouldn't recommend inheriting from this.
    /// </summary>
    public class State : MonoBehaviour
    {
        #region Config

        public new bool enabled
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        public bool locked = false;

        [FoldoutGroup("Lifetime Events", nameof(onAwakeEvent), nameof(onEnterEvent), nameof(onExitEvent), nameof(onUpdateEvent), nameof(onFixedUpdateEvent))]
        public Void lifetimeEventsHolder;

        [SerializeField, HideInInspector] public UltEvent<StateMachine> onAwakeEvent;
        [SerializeField, HideInInspector] public UltEvent<State> onEnterEvent;
        [SerializeField, HideInInspector] public UltEvent<State> onExitEvent;
        [SerializeField, HideInInspector] public UltEvent<float> onUpdateEvent;
        [SerializeField, HideInInspector] public UltEvent<float> onFixedUpdateEvent;

        #region Signals
        public SerializedDictionary<string, UltEvent> signals;
        public bool lockReady;
        #endregion 


        [Button("Add Sibling State")]
        protected virtual void AddSibling()
        {
            var NSGO = new GameObject("NewState");
            NSGO.transform.parent = base.transform.parent;
            NSGO.AddComponent<State>();
        }

        #endregion

        #region Data

        public StateMachine machine { get; protected set; }
        public StateBehaviour[] behaviors { get; protected set; }

        //Getters
        public StateBehaviour this[System.Type T] => GetComponent(T) as StateBehaviour;
        public T Behavior<T>() where T : StateBehaviour => behaviors.First(x => x is T) as T;
        public static implicit operator bool(State s) => s.enabled;

        #endregion


        public void Initialize(StateMachine machine)
        {
            this.machine = machine;

            enabled = false;

            behaviors = GetComponents<StateBehaviour>();
            for (int i = 0; i < behaviors.Length; i++) behaviors[i].Initialize(this);

            onAwakeEvent?.Invoke(machine);
        }

        public void DoUpdate()
        {
            for (int i = 0; i < behaviors.Length; i++) behaviors[i].OnUpdate();
            onUpdateEvent?.Invoke(Time.deltaTime);
        }
        public void DoFixedUpdate()
        {
            for (int i = 0; i < behaviors.Length; i++) behaviors[i].OnFixedUpdate();
            onFixedUpdateEvent?.Invoke(Time.fixedDeltaTime);
        }

        public State EnterState(State prev)
        {
            enabled = true;

            for (int i = 0; i < behaviors.Length; i++) behaviors[i].OnEnter(prev);

            machine.signalReady = !lockReady;
            onEnterEvent?.Invoke(prev);
            return this;
        }
        public void ExitState(State next)
        {
            enabled = false;

            for (int i = 0; i < behaviors.Length; i++) behaviors[i].OnExit(next);
            onExitEvent?.Invoke(next);
        }

        public void Enter() => machine.TransitionState(this);
    }
}