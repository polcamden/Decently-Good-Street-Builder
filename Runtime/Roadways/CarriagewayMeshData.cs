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
    }
}