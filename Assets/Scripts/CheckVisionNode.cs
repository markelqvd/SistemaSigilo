using UnityEngine;

public class CheckVisionNode : BTNode
{
    private EnemyVision vision;
    private Blackboard blackboard;

    public CheckVisionNode(EnemyVision _vision, Blackboard _blackboard)
    {
        vision = _vision;
        blackboard = _blackboard;
    }

    public override NodeState Evaluate()
    {
        if (vision.CanSeePlayer())
        {
            blackboard.isPlayerDetected = true;

            if (blackboard.playerTransform != null)
            {
                blackboard.targetPosition = blackboard.playerTransform.position;

                float distanciaAlPlayer = Vector3.Distance(vision.transform.position, blackboard.playerTransform.position);
                if (distanciaAlPlayer < 1.0f)
                {
                    if (GameMode.Instance != null)
                    {
                        GameMode.Instance.PlayerCapturado();
                    }
                }
            }

            state = NodeState.SUCCESS;
            return state;
        }

        blackboard.isPlayerDetected = false;
        state = NodeState.FAILURE;
        return state;
    }
}