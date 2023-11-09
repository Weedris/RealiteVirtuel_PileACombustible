using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeyboardActionManager : MonoBehaviour
{
    public static KeyboardActionManager Instance;


        
    

    private Rigidbody rb;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float sensitivity = 2f; // Sensibilité de la souris pour la rotation de la caméra

    private float rotationX = 0;

    private GameObject grabbedObject;
    private Rigidbody grabbedRigidbody;
    public Rigidbody pointDeGrab;
    private FixedJoint joint;

    public float maxGrabDistance = 5f;
    public Transform grabPoint; // Le point où l'objet sera attaché
    public float objectDistanceFromCamera = 2f; // Distance entre la caméra et l'objet saisi

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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

        //Grab Down
        if (Input.GetMouseButtonDown(0))
        {
            TryGrabObject();
        }

        //Grab Up
        if (Input.GetMouseButtonUp(0))
        {
            ReleaseObject();
        }
    }

    void Jump()
    {
        if (isGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool isGrounded()
    {
        Vector3 position = rb.gameObject.transform.position;
        return position.y < 1;
    }

    void TryGrabObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxGrabDistance))
        {
            if (hit.collider.gameObject.GetComponent<XRGrabInteractable>() != null)
            {
                GrabObject(hit.collider.gameObject);
            }
            else if (hit.collider.gameObject.GetComponent<XRSimpleInteractable>() != null)
            {
                GameManager.Instance.getMainComponentToTheirPlace();
            }
        }
    }

    void GrabObject(GameObject objToGrab)
    {
        grabbedObject = objToGrab;
        grabbedRigidbody = grabbedObject.GetComponent<Rigidbody>();

        // Désactivez la gravité
        grabbedRigidbody.useGravity = false;

        // Créez un joint
        joint = grabbedObject.AddComponent<FixedJoint>();
        joint.connectedBody = pointDeGrab; // Connectez l'objet saisi au joueur
        joint.breakForce = Mathf.Infinity; // Ajustez la résistance du joint si nécessaire
        joint.breakTorque = Mathf.Infinity;

        // Ajustez la position de l'objet pour le placer devant la caméra
        Vector3 offset = transform.forward * objectDistanceFromCamera;
        grabbedObject.transform.position = transform.position + offset;
        traceParser.Instance.traceInApp(grabbedObject);
        SoundManager.Instance.PlaySFX(SfxType.GrabbedObject);
    }

    public bool grabOn() { return (grabbedObject != null); }

    void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            // Réactivez la gravité
            grabbedRigidbody.useGravity = true;

            // Détruisez le joint
            Destroy(joint);

            grabbedObject = null;
        }
    }
}
