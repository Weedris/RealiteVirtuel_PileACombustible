using System.Collections;
using UnityEngine;

namespace Assets.Scripts.PEMFC
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class PhantomRenderer : MonoBehaviour
	{
		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private Material material;

		// variables controlling the visual effect of the phantom component. 
		[SerializeField] private float alphaMin = .5f;
		[SerializeField] private float alphaMax = .6f;
		[SerializeField] private float frequency = .5f;


		private void Start()
		{
			GetComponent<MeshRenderer>().material = material;
		}

		private void Update()
		{
			// creates a pulse effect
			Color newColor = material.color;
			float amplitude = alphaMax - alphaMin;
			newColor.a = alphaMin + (Mathf.Sin(Time.time * frequency * Mathf.PI) * amplitude + amplitude) / 2;
			material.color = newColor;
		}

		public void SetMesh(Mesh mesh)
		{
			meshFilter.mesh = mesh;
		}
	}
}