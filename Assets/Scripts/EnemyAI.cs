using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 4.5f;
    public List<Transform> patrolWaypoints = new List<Transform>();

    [Header("Configuración de Patrulla")]
    public float tiempoEsperaEnPunto = 2.0f; // Segundos que se quedará quieto en cada waypoint

    private Blackboard blackboard;
    private BTNode rootNode;
    private Pathfinding pathfinding;
    private EnemyVision vision;
    private bool estaRegistradoEnRadio = false;

    // Variables internas para el temporizador de espera
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

        // Árbol de Comportamiento estándar
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
        // Conexión automática a la radio
        if (!estaRegistradoEnRadio && AlertManager.Instance != null)
        {
            AlertManager.Instance.RegistrarGuardia(this);
            estaRegistradoEnRadio = true;
        }

        // 1. GESTIÓN DEL TIEMPO DE ESPERA EN WAYPOINTS
        if (estaEsperando && !blackboard.isPlayerDetected && !blackboard.alertReceived)
        {
            cronometroEspera += Time.deltaTime;
            if (cronometroEspera >= tiempoEsperaEnPunto)
            {
                estaEsperando = false;
                cronometroEspera = 0f;

                // Avanzamos de forma segura al siguiente punto (Bucle infinito garantizado)
                blackboard.currentWaypointIndex = (blackboard.currentWaypointIndex + 1) % blackboard.patrolWaypoints.Count;
                ActualizarDestinoPatrulla();
            }
            return; // Saltamos el resto del Update para que no se mueva mientras espera
        }

        // Ejecutamos el árbol de decisiones
        if (rootNode != null)
        {
            rootNode.Evaluate();
        }

        // SI VE AL JUGADOR: Alerta radial continua
        if (blackboard.isPlayerDetected)
        {
            estaEsperando = false; // Rompe la espera de inmediato si se activa la alarma
            if (AlertManager.Instance != null)
            {
                AlertManager.Instance.EnviarAlertaGlobal(blackboard.playerTransform.position, this);
            }
        }

        // SI NO VE AL JUGADOR (Lógica de patrulla y búsqueda)
        if (!blackboard.isPlayerDetected)
        {
            // Caso A: Patrulla normal (Sin alertas de radio pendientes)
            if (!blackboard.alertReceived && blackboard.patrolWaypoints.Count > 0)
            {
                Vector3 posicionPuntoActual = blackboard.patrolWaypoints[blackboard.currentWaypointIndex].position;

                // Subimos el margen de distancia a 0.8f para evitar que se quede atascado rozando el punto
                if (Vector3.Distance(transform.position, posicionPuntoActual) < 0.8f)
                {
                    estaEsperando = true; // Activamos el temporizador
                    cronometroEspera = 0f;
                    Debug.Log("<color=orange>" + transform.name + "</color>: Inspeccionando zona de patrulla...");
                }
            }

            // Caso B: El enemigo ha llegado a la "Última Posición Registrada" del jugador o a la radio de alerta
            // Si llega al punto sospechoso y ya no ve al jugador, se activa esta limpieza
            float distanciaAlObjetivo = Vector3.Distance(transform.position, blackboard.targetPosition);

            // Si está muy cerca del último avistamiento y sigue sin ver a nadie...
            if (distanciaAlObjetivo < 1.2f)
            {
                if (blackboard.alertReceived)
                {
                    blackboard.alertReceived = false;
                    Debug.Log("<color=cyan>" + transform.name + "</color>: Falsa alarma radial. Reanudando patrulla.");
                }
                else
                {
                    Debug.Log("<color=yellow>" + transform.name + "</color>: Perdí al jugador en su última posición. Regresando a mi ruta habitual.");
                }

                // CORRECCIÓN: Le obligamos a limpiar su memoria y volver a fijar su waypoint de patrulla
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

            Debug.Log("<color=cyan>" + transform.name + ":</color> ¡Recibido! Voy hacia la posición de alerta.");
        }
    }
}