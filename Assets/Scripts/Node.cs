using UnityEngine;

public class Node
{
    public bool isWalkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public bool isDark;

    public int gCost;
    public int hCost;
    public Node parent;

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node(bool _isWalkable, Vector3 _worldPos, int _gridX, int _gridY, bool _isDark)
    {
        isWalkable = _isWalkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        isDark = _isDark;
    }
}