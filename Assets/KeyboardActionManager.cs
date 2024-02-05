using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeyboardActionManager : MonoBehaviour
{
	public static KeyboardActionManager Instance;

	private Rigidbody rb;
	public float moveSpeed = 3f;
	public float jumpForce = 5f;
	public GameObject ground;
    public float sensitivity = 2f; // Sensibilit� de la souris pour la rotation de la cam�ra

	private float rotationX = 0;

	private GameObject grabbedObject;
	private Rigidbody grabbedRigidbody;
	public Rigidbody pointDeGrab;
	private FixedJoint joint;

	public float maxGrabDistance = 5f;
	public Transform grabPoint; // Le point o� l'objet sera attach�
	public float objectDistanceFromCamera = 2f; // Distance entre la cam�ra et l'objet saisi
	private Vector3 objectScrollVector = new Vector3(0, 0, 0);

	private bool isCrouching = false; // Variable pour suivre l'�tat d'accroupissement
	public float crouchSpeed = 1.5f; // Vitesse pendant l'accroupissement
	public float crouchHeight = 0.3f; // Hauteur pendant l'accroupissement
	private float originalHeight; // Hauteur originale du joueur

	private void Awake()
	{
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		
		Cursor.lockState = CursorLockMode.Locked; // Verrouille le curseur au centre de l'�cran
		originalHeight = Camera.main.transform.localPosition.y; // Stockez la hauteur originale du joueur
	}

	void Update()
	{
		// Mouvement de la souris pour la rotation de la cam�ra
		float mouseX = Input.GetAxis("Mouse X") * sensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

		rotationX -= mouseY;
		rotationX = Mathf.Clamp(rotationX, -90, 90); // Limite la rotation verticale

		transform.Rotate(Vector3.up * mouseX);
		Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

		// Mouvement du joueur
		float horizontalInput = Input.GetAxis("Horizontal");
		float verticalInput = Input.GetAxis("Vertical");

		Vector3 movement = Vector3.zero;

		if (!isCrouching)
		{
			movement = new Vector3(horizontalInput, 0, verticalInput) * moveSpeed * Time.deltaTime;
		}
		else
		{
			movement = new Vector3(horizontalInput, 0, verticalInput) * crouchSpeed * Time.deltaTime;
		}

		rb.MovePosition(rb.position + transform.TransformDirection(movement));
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

		//Crouch
		if (Input.GetKey(KeyCode.LeftShift))
		{
			Crouch();
		}
		else
		{
			StandUp();
		}

		// D�tection du roulement de la molette de la souris
		float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
		if (scrollWheel != 0f)
		{
			if (GrabOn())
			{
				objectScrollVector.z = Mathf.Clamp(objectScrollVector.z + scrollWheel, -0.5f, 1f);
				grabPoint.localPosition = objectScrollVector;
			}
		}
	}

	void Jump()
	{
		if (IsGrounded())
		{
			rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}
	}

	private bool IsGrounded()
	{
        Collider playerCollider = GetComponent<Collider>();
        Collider groundCollider = ground.GetComponent<Collider>();

        // V�rifier la collision en utilisant les m�thodes d'Unity
        return playerCollider.bounds.Intersects(groundCollider.bounds);
    }

	void Crouch()
	{
		if (!isCrouching)
		{
			Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, crouchHeight, Camera.main.transform.localPosition.z);
			rb.GetComponent<CapsuleCollider>().height /= 1.5f;
			isCrouching = true;
		}
	}

	void StandUp()
	{
		if (isCrouching)
		{
			Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, originalHeight, Camera.main.transform.localPosition.z);
			rb.GetComponent<CapsuleCollider>().height *= 1.5f;
			isCrouching = false;
		}
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

		// Cr�ez un joint
		joint = grabbedObject.AddComponent<FixedJoint>();
		joint.connectedBody = pointDeGrab; // Connectez l'objet saisi au joueur
		joint.breakForce = Mathf.Infinity; // Ajustez la r�sistance du joint si n�cessaire
		joint.breakTorque = Mathf.Infinity;

		traceParser.Instance.traceInApp(grabbedObject);
		SoundManager.Instance.PlaySFX(SfxType.GrabbedObject);
	}

	public bool GrabOn() { return (grabbedObject != null); }

	void ReleaseObject()
	{
		if (grabbedObject != null)
		{
			// D�truisez le joint
			Destroy(joint);

			objectScrollVector.z = 0;
			grabbedObject = null;
		}
	}
}
