using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{
    public Vector3 targetPosition;
    public Transform playerTransform;
    public bool isPlayerDetected = false;

    public bool alertReceived = false;
    public Vector3 alertPosition;

    public List<Transform> patrolWaypoints = new List<Transform>();
    public int currentWaypointIndex = 0;
}