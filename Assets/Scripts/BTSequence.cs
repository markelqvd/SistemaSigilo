using System.Collections.Generic;

public class BTSequence : BTNode
{
    public BTSequence(List<BTNode> children)
    {
        foreach (BTNode child in children)
            Attach(child);
    }

    public override NodeState Evaluate()
    {
        bool anyChildRunning = false;

        foreach (BTNode child in children)
        {
            switch (child.Evaluate())
            {
                case NodeState.FAILURE:
                    state = NodeState.FAILURE;
                    return state;
                case NodeState.SUCCESS:
                    continue;
                case NodeState.RUNNING:
                    anyChildRunning = true;
                    continue;
            }
        }
        state = anyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return state;
    }
}