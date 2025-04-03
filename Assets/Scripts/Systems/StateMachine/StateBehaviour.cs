using UnityEngine;

namespace ObjectBasedStateMachine.UnLayered
{
    /// <summary>
    /// Behavior Scripts attached to a state. Inherit from this to create functionality.
    /// </summary>
    [RequireComponent(typeof(State))]
    public abstract class StateBehaviour : MonoBehaviour
    {

        /// <summary>
        /// The State Machine owning this behavior. Likely the most important field you'll be referencing a lot.<br />
        /// Override with the "new" keyword with an expression like "=> M as MyStateMachine" to get a custom StateMachine
        /// </summary>
        public StateMachine Machine { get; private set; }
        /// <summary>
        /// An indirection to access the State Machine's gameObject property.
        /// </summary>
        public new GameObject gameObject => Machine.gameObject;
        /// <summary>
        /// An indirection to access the State Machine's transform property.
        /// </summary>
        public new Transform transform => Machine.transform;
        /// <summary>
        /// The current State. Usefull for referencing this SubObject.
        /// </summary>
        public State state { get; private set; }


        public void Initialize(State @state)
        {
            Machine = @state.machine;
            this.state = @state;

            this.OnAwake();
        }


        public virtual void OnAwake() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnEnter(State prev) { }
        public virtual void OnExit(State next) { }

        public virtual void Enter() => state.Enter();


        public static implicit operator bool(StateBehaviour B) => B != null && B.state.enabled;
    }
}