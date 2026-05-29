using System.Collections.Generic;

public enum NodeState { RUNNING, SUCCESS, FAILURE }

public abstract class BTNode
{
    protected NodeState state;
    public BTNode parent;
    protected List<BTNode> children = new List<BTNode>();

    public BTNode() { parent = null; }

    public void Attach(BTNode child)
    {
        child.parent = this;
        children.Add(child);
    }

    public abstract NodeState Evaluate();
}