using UnityEngine;

namespace DecentlyGoodStreetBuilder.Roadway {
	public class CarriagewayMesh : RoadwayPart, IRoadwayMesh
	{
		[SerializeField] public bool collidable;
		[SerializeField] public Material material;

		public bool Collidable {
			get { return collidable; }
		}

		public Material Material
		{
			get { return material; }
		}

		public Mesh GenerateMesh(CubicBezierCurve curve, RoadwayData data)
		{
			CarriagewayMeshData castedData = (CarriagewayMeshData)data;

			Vector2[] roadSlice = new Vector2[1];

			return null;
		}
	}
}