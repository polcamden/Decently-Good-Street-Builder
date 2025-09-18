using UnityEngine;
using System;

namespace DecentlyGoodStreetBuilder.Roadway {
	public class CarriagewayMesh : RoadwayPart, IRoadwayMesh
	{
		[SerializeField] public bool collidable;
		[SerializeField] public Material material;

		[Range(0, 90)]
		[SerializeField] private float shoulderAngle;
		[SerializeField] private float shoulderWidth;

		public bool Collidable {
			get { return collidable; }
		}

		public Material Material
		{
			get { return material; }
		}

		public override Type DataClass
		{
			get { return typeof(MeshArrayData); }
		}

		public Mesh GenerateMesh(CubicBezierCurve curve, RoadwayData data)
		{
			CarriagewayMeshData castedData = (CarriagewayMeshData)data;

			Vector2[] roadSlice = new Vector2[1];



			return null;
		}
	}
}