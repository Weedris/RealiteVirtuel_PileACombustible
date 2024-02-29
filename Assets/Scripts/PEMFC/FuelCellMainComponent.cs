using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts.PEMFC
{
	public class FuelCellMainComponent : MonoBehaviour
	{
		[Tooltip("The components (secondary) that are shown when this component is placed correctly within the fuel cell")]
		public GameObject[] ToShowWhenPlaced;
		public FuelCellComponent WhoAmI;
		public Mesh SimplifiedShape;

		private Vector3 initialPosition;
		private Quaternion initialRotation;

		private List<FuelCellSocket> touchingSockets = new();


		private void Awake()
		{
			initialPosition = transform.position;
			initialRotation = transform.rotation;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.TryGetComponent(out FuelCellSocket socket))
				touchingSockets.Add(socket);
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.gameObject.TryGetComponent(out FuelCellSocket socket))
				touchingSockets.Remove(socket);
		}

		public void TryPlace()
		{
			if (touchingSockets.Count == 0)
				return;

			// if multiple exists, automatically find the nearest socket
			FuelCellSocket nearest = touchingSockets.OrderBy((socket) => Vector3.Distance(transform.position, socket.transform.position)).FirstOrDefault();
			AssemblyGameManager.Instance.AttemptPlaceObject(this, nearest);
		}

		public void Place(FuelCellSocket socket)
		{
			// position
			transform.parent = socket.transform;
			transform.SetLocalPositionAndRotation(Vector3.zero, new Quaternion());

			// remove possible future interactions
			Destroy(socket.GetComponent<Collider>());
			Destroy(GetComponent<XRGrabInteractable>());
			Destroy(GetComponent<Rigidbody>());
			Destroy(GetComponent<Collider>());

			foreach (GameObject go in ToShowWhenPlaced)
				go.SetActive(true);
		}

		public void ResetPositionAndRotation()
		{
			transform.SetLocalPositionAndRotation(initialPosition, initialRotation);			
		}
	}
}