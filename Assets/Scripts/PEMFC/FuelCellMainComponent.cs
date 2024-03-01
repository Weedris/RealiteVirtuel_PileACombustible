﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts.PEMFC
{
	public class FuelCellMainComponent : MonoBehaviour
	{
		[Tooltip("The components (secondary) that are shown when this component is placed correctly within the fuel cell")]
		public GameObject[] ToShowWhenPlaced;
		public FuelCellComponent WhoAmI;
		[FormerlySerializedAs("SimplifiedShape")] public Mesh SimplifiedMesh;

		private Vector3 initialPosition;
		private Quaternion initialRotation;

		private List<FuelCellSocket> touchingSockets = new();
		private bool isPlaced = false;


		private void Awake()
		{
			initialPosition = transform.localPosition;
			initialRotation = transform.localRotation;
		}

		private void OnEnable()
		{
			GetComponent<XRGrabInteractable>().lastSelectExited.AddListener(XRGrabEnd);
		}

		private void XRGrabEnd(SelectExitEventArgs args)
		{
			DataSaver.Instance.Log($"[Debug] {name} released with args {args}");
			TryPlace();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.TryGetComponent(out FuelCellSocket socket))
			{
				touchingSockets.Add(socket);
				socket.CreatePhantom(this);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.gameObject.TryGetComponent(out FuelCellSocket socket))
			{
				touchingSockets.Remove(socket);
				socket.RemovePhantom();
			}
		}

		public void TryPlace()
		{
			if (touchingSockets.Count == 0)
				return;

			// if multiple exists, automatically find the nearest socket
			FuelCellSocket nearest = touchingSockets.OrderBy((socket) => Vector3.Distance(transform.position, socket.transform.position)).FirstOrDefault();
			AssemblyGameManager.Instance.AttemptPlaceObject(this, nearest);
		}

		/// <summary>
		/// Snaps the component in the specified socket and prevent any further interaction.
		/// The reason being that correctly placing a component should be definitive.
		/// It also helps reduce the number of calculation i guess.
		/// </summary>
		/// <param name="socket">The socket that will host the component</param>
		public void Place(FuelCellSocket socket)
		{
			// prevent any change in position that we don't want
			GetComponent<Rigidbody>().useGravity = false;

			// remove phantom components (the thing that help players see where they will place the component)
			foreach (var touchingSocket in touchingSockets)
				touchingSocket.RemovePhantom();

			// snap the component where we want it to be
			transform.parent = socket.transform;
			transform.SetLocalPositionAndRotation(Vector3.zero, new Quaternion());

			// remove possible future interactions
			isPlaced = true;
			Deactivate();
			socket.Deactivate();

			// display cables and stuff that connects to this component
			foreach (GameObject go in ToShowWhenPlaced)
				go.SetActive(true);
		}

		private void Deactivate()
		{
			Destroy(GetComponent<XRGrabInteractable>());
			Destroy(GetComponent<Rigidbody>());
			Destroy(GetComponent<Collider>());
		}

		public void ResetPositionAndRotation()
		{
			if (!isPlaced)
				transform.SetLocalPositionAndRotation(initialPosition, initialRotation);
		}
	}
}