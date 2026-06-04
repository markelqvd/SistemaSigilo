using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMode : MonoBehaviour
{
    public static GameMode Instance { get; private set; }

    [Header("Puntos de Control")]
    public Transform respawnPoint;
    public GameObject pantallaVictoriaUI;

    private GameObject player;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (pantallaVictoriaUI != null) pantallaVictoriaUI.SetActive(false);

        RespawnPlayer();
    }

    public void PlayerCapturado()
    {
        Debug.Log("GAME OVER");
        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        if (player != null && respawnPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = respawnPoint.position;
            player.transform.rotation = respawnPoint.rotation;

            if (cc != null) cc.enabled = true;

            if (AlertManager.Instance != null)
            {
                AlertManager.Instance.ResetearAlertasGlobales();
            }

            Debug.Log("Jugador devuelto al punto de inicio.");
        }
    }

    public void Victoria()
    {
        Debug.Log("VICTORIA");
        if (pantallaVictoriaUI != null)
        {
            pantallaVictoriaUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}