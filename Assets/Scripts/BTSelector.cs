using System.Collections.Generic;

public class BTSelector : BTNode
{
    public BTSelector(List<BTNode> children)
    {
        foreach (BTNode child in children)
            Attach(child);
    }

    public override NodeState Evaluate()
    {
        foreach (BTNode child in children)
        {
            switch (child.Evaluate())
            {
                case NodeState.FAILURE:
                    continue;
                case NodeState.SUCCESS:
                    state = NodeState.SUCCESS;
                    return state;
                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    return state;
            }
        }
        state = NodeState.FAILURE;
        return state;
    }
}