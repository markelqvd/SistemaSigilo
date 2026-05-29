using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Configuración de Visión")]
    public float viewRadius = 8f;
    public float viewAngle = 90f;
    public float meleeRadius = 2f;

    [Header("Filtros de Capas")]
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    private Transform playerTransform;
    private PlayerController playerScript;

    void Start()
    {
        playerScript = Object.FindFirstObjectByType<PlayerController>();
        if (playerScript != null)
        {
            playerTransform = playerScript.transform;
        }
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            Debug.Log("<color=red>¡TE VEO!</color> El jugador está al descubierto.");
        }
    }

    public bool CanSeePlayer()
    {
        if (playerTransform == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < viewRadius)
        {
            Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask))
                {
                    if (playerScript.IsInDarkZone)
                    {
                        return distanceToPlayer < meleeRadius;
                    }
                    return true;
                }
            }
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}