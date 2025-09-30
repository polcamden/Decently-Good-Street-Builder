using UnityEngine;

namespace DecentlyGoodStreetBuilder.Roadway
{
    public class CarriagewayMeshData : RoadwayData
    {
        [SerializeField] public float width = 6;
        [SerializeField] public bool enableMargins = true;
		[Range(0.01f, 10f)]
		[SerializeField] public float marginSize = 3;
		[Range(0, 90)]
		[SerializeField] public float marginDegree = 45;

		public Vector2[] CrossSectionPoints()
		{
			float halfWidth = width / 2;
			float angle = Mathf.Deg2Rad * -marginDegree;
			Vector2[] roadSlice;

			if (enableMargins)
			{
				roadSlice = new Vector2[4];

				roadSlice[1] = new Vector2(-halfWidth, 0);
				roadSlice[2] = new Vector2(halfWidth, 0);
				roadSlice[0] = new Vector2(-Mathf.Cos(angle) * marginSize, Mathf.Sin(angle) * marginSize) + roadSlice[1];
				roadSlice[3] = new Vector2(Mathf.Cos(angle) * marginSize, Mathf.Sin(angle) * marginSize) + roadSlice[2];
			}
			else
			{
				roadSlice = new Vector2[2];
				roadSlice[0] = new Vector2(-halfWidth, 0);
				roadSlice[1] = new Vector2(halfWidth, 0);
			}

			return roadSlice;
		}
	}
}