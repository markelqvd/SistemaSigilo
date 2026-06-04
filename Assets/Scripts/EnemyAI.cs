using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 4.5f;
    public List<Transform> patrolWaypoints = new List<Transform>();

    [Header("Configuración de Patrulla")]
    public float tiempoEsperaEnPunto = 2.0f;

    private Blackboard blackboard;
    private BTNode rootNode;
    private Pathfinding pathfinding;
    private EnemyVision vision;
    private bool estaRegistradoEnRadio = false;

    private float cronometroEspera = 0f;
    private bool estaEsperando = false;

    void Start()
    {
        pathfinding = GetComponent<Pathfinding>();
        vision = GetComponent<EnemyVision>();

        blackboard = new Blackboard();

        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null) blackboard.playerTransform = player.transform;

        blackboard.patrolWaypoints = patrolWaypoints;
        blackboard.currentWaypointIndex = 0;

        ActualizarDestinoPatrulla();

        BTSequence chaseSequence = new BTSequence(new List<BTNode>
        {
            new CheckVisionNode(vision, blackboard),
            new MoveToTargetNode(transform, pathfinding, blackboard, chaseSpeed)
        });

        BTSequence patrolSequence = new BTSequence(new List<BTNode>
        {
            new MoveToTargetNode(transform, pathfinding, blackboard, patrolSpeed)
        });

        rootNode = new BTSelector(new List<BTNode> { chaseSequence, patrolSequence });
    }

    void Update()
    {
        if (!estaRegistradoEnRadio && AlertManager.Instance != null)
        {
            AlertManager.Instance.RegistrarGuardia(this);
            estaRegistradoEnRadio = true;
        }

        if (estaEsperando && !blackboard.isPlayerDetected && !blackboard.alertReceived)
        {
            cronometroEspera += Time.deltaTime;
            if (cronometroEspera >= tiempoEsperaEnPunto)
            {
                estaEsperando = false;
                cronometroEspera = 0f;

                blackboard.currentWaypointIndex = (blackboard.currentWaypointIndex + 1) % blackboard.patrolWaypoints.Count;
                ActualizarDestinoPatrulla();
            }
            return;
        }

        if (rootNode != null)
        {
            rootNode.Evaluate();
        }

        if (blackboard.isPlayerDetected)
        {
            estaEsperando = false;
            if (AlertManager.Instance != null)
            {
                AlertManager.Instance.EnviarAlertaGlobal(blackboard.playerTransform.position, this);
            }
        }

        if (!blackboard.isPlayerDetected)
        {
            if (!blackboard.alertReceived && blackboard.patrolWaypoints.Count > 0)
            {
                Vector3 posicionPuntoActual = blackboard.patrolWaypoints[blackboard.currentWaypointIndex].position;

                if (Vector3.Distance(transform.position, posicionPuntoActual) < 0.8f)
                {
                    estaEsperando = true; // Activamos el temporizador
                    cronometroEspera = 0f;
                    Debug.Log(transform.name + ": Inspeccionando zona de patrulla");
                }
            }

            float distanciaAlObjetivo = Vector3.Distance(transform.position, blackboard.targetPosition);

            if (distanciaAlObjetivo < 1.2f)
            {
                if (blackboard.alertReceived)
                {
                    blackboard.alertReceived = false;
                    Debug.Log(transform.name + ": Falsa alarma. Reanudando patrulla.");
                }
                else
                {
                    Debug.Log("Regresando a mi ruta habitual.");
                }

                ActualizarDestinoPatrulla();
            }
        }
    }

    void ActualizarDestinoPatrulla()
    {
        if (blackboard.patrolWaypoints != null && blackboard.patrolWaypoints.Count > 0)
        {
            blackboard.targetPosition = blackboard.patrolWaypoints[blackboard.currentWaypointIndex].position;
        }
    }

    public void RecibirAlertaDeCompañero(Vector3 posicionDeLaAmenaza)
    {
        if (!blackboard.isPlayerDetected)
        {
            blackboard.alertReceived = true;
            blackboard.alertPosition = posicionDeLaAmenaza;

            blackboard.targetPosition = posicionDeLaAmenaza;

            Debug.Log(transform.name + "Voy hacia la posición de alerta.");
        }
    }

    public void OlvidarIntrusoYVolverAPatrulla()
    {
        if (blackboard != null)
        {
            blackboard.isPlayerDetected = false;
            blackboard.alertReceived = false;
            estaEsperando = false;
            cronometroEspera = 0f;

            ActualizarDestinoPatrulla();
        }
    }
}