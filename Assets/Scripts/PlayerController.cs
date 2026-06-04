using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public LayerMask darkZoneMask;
    public bool IsInDarkZone { get; private set; }

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        if (moveDirection != Vector3.zero)
        {
            controller.Move(moveDirection * speed * Time.deltaTime);

            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
        }

        IsInDarkZone = Physics.CheckSphere(transform.position, 0.4f, darkZoneMask);
    }
}