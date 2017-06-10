using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{

    [Action("Navigation/MoveToGameObject")]
    [Help("Moves the game object towards a given target by using a NavMeshAgent")]
    public class MoveToGameObject : GOAction
    {
        [InParam("target")]
        [Help("Target game object towards this game object will be moved")]
        public GameObject target;

        private UnityEngine.AI.NavMeshAgent navAgent;

        private Transform targetTransform;

        public override void OnStart()
        {
            if (target == null)
            {
                Debug.LogError("The movement target of this game object is null", gameObject);
                return;
            }
            targetTransform = target.transform;

            navAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navAgent == null)
            {
                Debug.LogWarning("The " + gameObject.name + " game object does not have a Nav Mesh Agent component to navigate. One with default values has been added", gameObject);
                navAgent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
            }
			navAgent.SetDestination(targetTransform.position);
            navAgent.Resume();
        }

        public override TaskStatus OnUpdate()
        {
            if (target == null)
                return TaskStatus.FAILED;
            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
                return TaskStatus.COMPLETED;
            else if (navAgent.destination != targetTransform.position)
                navAgent.SetDestination(targetTransform.position);
            return TaskStatus.RUNNING;
        }

        public override void OnAbort()
        {
            navAgent.Stop();
        }
    }
}
