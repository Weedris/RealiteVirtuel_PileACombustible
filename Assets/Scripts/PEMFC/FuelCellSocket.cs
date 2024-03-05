using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PEMFC
{
	/// <summary>
	/// This needs to be placed on GameObjects that will receive a <see cref="FuelCellComponent"/>.
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class FuelCellSocket : MonoBehaviour
	{
		[Tooltip("Specific FuelCellComponent that this socket can receive.")]
		public FuelCellComponentType Target;

		[SerializeField] private GameObject phatomRendererPrefab;
		private List<GameObject> phantomGameObjects = new();

		public void CreatePhantom(FuelCellComponent component)
		{
			GameObject phantomGameObject = Instantiate(phatomRendererPrefab, transform);
			phantomGameObject.GetComponent<PhantomRenderer>().SetMesh(component.SimplifiedMesh);
			phantomGameObjects.Add(phantomGameObject);
		}

		public void RemovePhantom()
		{
            foreach (GameObject phantomGameObject in phantomGameObjects)
				Destroy(phantomGameObject);
			phantomGameObjects.Clear();
        }

		/// <summary>
		/// Destroys every unused components
		/// Used when a component place itself in the socket to delete phantom shapes
		/// </summary>
		public void Deactivate()
		{
			if (phantomGameObjects.Count != 0)
				RemovePhantom();
			Destroy(this);
			Destroy(GetComponent<Collider>());
		}
	}
}
