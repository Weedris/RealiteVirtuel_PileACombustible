using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardActionManager : MonoBehaviour
{
    private Rigidbody rb;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float sensitivity = 2f; // Sensibilité de la souris pour la rotation de la caméra

    private float rotationX = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // Verrouille le curseur au centre de l'écran
    }

    void Update()
    {
        // Mouvement de la souris pour la rotation de la caméra
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90, 90); // Limite la rotation verticale

        transform.Rotate(Vector3.up * mouseX);
        Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // Mouvement du joueur
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);

        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        // Saut
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
