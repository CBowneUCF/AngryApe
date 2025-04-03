using ObjectBasedStateMachine.UnLayered;
using UnityEngine;

namespace ObjectBasedStateMachine
{
    [RequireComponent(typeof(Rigidbody), typeof(StateMachine))]
    public class StatePhysicsCaller : MonoBehaviour
    {
        private StateBehaviorPhysicsCollision[] collisions;
        private StateBehaviorPhysicsTrigger[] triggers;
        private StateMachine Machine;

        protected void Awake()
        {
            Machine = GetComponent<StateMachine>();
            collisions = Machine.stateHolder.GetComponentsInChildren<StateBehaviorPhysicsCollision>();
            triggers = Machine.stateHolder.GetComponentsInChildren<StateBehaviorPhysicsTrigger>();
        }


        private void OnCollisionEnter(Collision collision)
        {
            for (int i = 0; i < collisions.Length; i++)
                if (collisions[i].isActive())
                    collisions[i].OnCollisionEnter(collision);
        }
        private void OnCollisionExit(Collision collision)
        {
            for (int i = 0; i < collisions.Length; i++)
                if (collisions[i].isActive())
                    collisions[i].OnCollisionExit(collision);
        }
        private void OnTriggerEnter(Collider other)
        {
            for (int i = 0; i < triggers.Length; i++)
                if (triggers[i].isActive())
                    triggers[i].OnTriggerEnter(other);

        }
        private void OnTriggerExit(Collider other)
        {
            for (int i = 0; i < triggers.Length; i++)
                if (triggers[i].isActive())
                    triggers[i].OnTriggerExit(other);
        }
    }

}

