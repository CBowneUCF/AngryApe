using ObjectBasedStateMachine.UnLayered;
using UnityEngine;

namespace ObjectBasedStateMachine
{
    public interface StateBehaviorPhysics
    {
        public sealed bool isActive() => (this as StateBehaviour).state.enabled;
    }
    public interface StateBehaviorPhysicsCollision : StateBehaviorPhysics
    {
        void OnCollisionEnter(Collision collision);
        void OnCollisionExit(Collision collision);
    }
    public interface StateBehaviorPhysicsTrigger : StateBehaviorPhysics
    {
        void OnTriggerEnter(Collider other);
        void OnTriggerExit(Collider other);
    }
    public interface StateBehaviorPhysicsCollision2D : StateBehaviorPhysics
    {
        void OnCollisionEnter2D(Collision2D collision);
        void OnCollisionExit2D(Collision2D collision);
    }
    public interface StateBehaviorPhysicsTrigger2D : StateBehaviorPhysics
    {
        void OnTriggerEnter2D(Collider2D collision);
        void OnTriggerExit2D(Collider2D collision);
    }
}