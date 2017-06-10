using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{

    [Action("Navigation/MoveToPosition")]
    [Help("Moves the game object to a given position by using a NavMeshAgent")]
    public class MoveToPosition : GOAction
    {
        [InParam("target")]
        [Help("Target position where the game object will be moved")]
        public Vector3 target;

        private UnityEngine.AI.NavMeshAgent navAgent;

        public override void OnStart()
        {
            navAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navAgent == null)
            {
                Debug.LogWarning("The " + gameObject.name + " game object does not have a Nav Mesh Agent component to navigate. One with default values has been added", gameObject);
                navAgent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
            }
            navAgent.SetDestination(target);
            navAgent.Resume();
        }

        public override TaskStatus OnUpdate()
        {
            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
                return TaskStatus.COMPLETED;

            return TaskStatus.RUNNING;
        }

        public override void OnAbort()
        {
            navAgent.Stop();
        }
    }
}
