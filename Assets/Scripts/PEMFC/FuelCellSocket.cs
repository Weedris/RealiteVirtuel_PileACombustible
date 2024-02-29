using UnityEngine;

namespace Assets.Scripts.PEMFC
{
	public class FuelCellSocket : MonoBehaviour
	{
		[Tooltip("The component the socket can receive")]
		public FuelCellComponent Target;
		[Tooltip("The material of the simplified shape of a main component entering the socket")]
		[SerializeField] private Material phantomMaterial;

		private GameObject phantomComponent;
		private MeshFilter phantomComponentMeshFilter;

		// animation of the phantom component
		private const float alphaMin = .5f;
		private const float alphaMax = .6f;
		private const float frequency = .5f;

		private void Awake()
		{
			phantomComponent = new GameObject("PhantomComponent", typeof(MeshRenderer), typeof(MeshFilter));
			phantomComponentMeshFilter = phantomComponent.GetComponent<MeshFilter>();
			phantomComponent.transform.parent = transform;
		}

		private void Update()
		{
			// phantom component animation
			if (phantomComponent.activeSelf)
			{
				Color newColor = phantomMaterial.color;
				float amplitude = alphaMax - alphaMin;
				newColor.a = alphaMin + (Mathf.Sin(Time.time * frequency * Mathf.PI) * amplitude + amplitude) / 2;
				phantomMaterial.color = newColor;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent(out FuelCellMainComponent component))
			{
				phantomComponentMeshFilter.mesh = component.SimplifiedShape;
				phantomComponent.SetActive(true);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.TryGetComponent<FuelCellMainComponent>(out _))
				phantomComponent.SetActive(false);
		}

	}
}