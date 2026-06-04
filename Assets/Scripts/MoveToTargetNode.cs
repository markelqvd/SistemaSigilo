using UnityEngine;
using System.Collections.Generic;

public class MoveToTargetNode : BTNode
{
    private Transform transform;
    private Pathfinding pathfinding;
    private Blackboard blackboard;
    private float speed = 3.5f;

    private List<Node> currentPath = new List<Node>();
    private int currentPathIndex = 0;
    private Vector3 lastTargetPos;

    public MoveToTargetNode(Transform _transform, Pathfinding _pathfinding, Blackboard _blackboard, float _speed)
    {
        transform = _transform;
        pathfinding = _pathfinding;
        blackboard = _blackboard;
        speed = _speed;
        lastTargetPos = Vector3.positiveInfinity;
    }
    public override NodeState Evaluate()
    {
        if (pathfinding == null)
        {
            pathfinding = transform.GetComponent<Pathfinding>();
        }

        if (pathfinding == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        Vector3 targetPos = blackboard.targetPosition;

        if (Vector3.Distance(targetPos, lastTargetPos) > 0.5f)
        {
            lastTargetPos = targetPos;
            currentPath = pathfinding.FindPath(transform.position, targetPos);
            currentPathIndex = 0;
        }

        if (currentPath == null || currentPath.Count == 0)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (currentPathIndex < currentPath.Count)
        {
            Vector3 targetNodePos = currentPath[currentPathIndex].worldPosition;
            targetNodePos.y = transform.position.y;

            transform.position = Vector3.MoveTowards(transform.position, targetNodePos, speed * Time.deltaTime);

            Vector3 direction = (targetNodePos - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, targetNodePos) < 0.2f)
            {
                currentPathIndex++;
            }

            state = NodeState.RUNNING;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }
}