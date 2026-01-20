using UnityEngine;
using System;

namespace DecentlyGoodStreetBuilder.Roadway {
	[CreateAssetMenu(menuName = "Decently Good Street Builder/Carriageway Mesh", order = 50)]
	public class CarriagewayMesh : RoadwayPart, IRoadwayMesh
	{
		[SerializeField] public bool collidable;
		[SerializeField] private Material material;
		[SerializeField] private float textureScale;
		[SerializeField] private float resolution;

		public bool Collidable {
			get { return collidable; }
		}

		public Material Material
		{
			get { return material; }
		}

		public override Type DataClass
		{
			get { return typeof(CarriagewayMeshData); }
		}

		public Mesh GenerateMesh(CubicBezierCurve curve, RoadwayData data)
		{
			CarriagewayMeshData castedData = (CarriagewayMeshData)data;
			float halfWidth = castedData.width / 2;
			float angle = Mathf.Deg2Rad * -castedData.marginDegree;
			float marginSize = castedData.marginSize;
			CubicBezierCurve offsetCurve = curve.offsetCurve(data.offset);

			Vector2[] roadSlice;
			bool[] sharpVerts;
			float[] vAxis;

			if (castedData.enableMargins)
			{
				roadSlice = new Vector2[4];
				sharpVerts = new bool[4];
				vAxis = new float[4];

				roadSlice[1] = new Vector2(-halfWidth, 0);
				roadSlice[2] = new Vector2(halfWidth, 0);

				roadSlice[0] = new Vector2(-Mathf.Cos(angle) * marginSize, Mathf.Sin(angle) * marginSize) + roadSlice[1];
				roadSlice[3] = new Vector2(Mathf.Cos(angle) * marginSize, Mathf.Sin(angle) * marginSize) + roadSlice[2];

				vAxis[0] = -halfWidth - marginSize;
				vAxis[1] = -halfWidth;
				vAxis[2] = halfWidth;
				vAxis[3] = halfWidth + marginSize;
			}
			else
			{
				roadSlice = new Vector2[2];
				sharpVerts = new bool[2];
				vAxis = new float[2];

				roadSlice[0] = new Vector2(-halfWidth, 0);
				roadSlice[1] = new Vector2(halfWidth, 0);

				vAxis[0] = -halfWidth;
				vAxis[1] = halfWidth;
			}


			return MeshArray.GenerateMeshGivenSlice(offsetCurve, resolution, roadSlice, sharpVerts, vAxis, resolution * textureScale);
		}
	}
}