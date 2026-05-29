using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public LayerMask darkZoneMask;
    public bool IsInDarkZone { get; private set; }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
        }

        IsInDarkZone = Physics.CheckSphere(transform.position, 0.4f, darkZoneMask);
    }
}