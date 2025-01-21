using DecentlyGoodStreetBuilder.Roadway;
using UnityEngine;

namespace DecentlyGoodStreetBuilder.Roadway
{
    [System.Serializable]
    public class MeshArrayData : RoadwayData
    {
        [SerializeField] public bool mirror;
        [SerializeField] public float resolution = 1;
    }
}