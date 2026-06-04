using UnityEngine;

public class MetaTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
        {
            if (GameMode.Instance != null)
            {
                GameMode.Instance.Victoria();
            }
        }
    }
}