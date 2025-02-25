using System;
using System.Collections.Generic;
using UnityEngine;

namespace DecentlyGoodStreetBuilder.Roadway
{
    public abstract class RoadwayPart : ScriptableObject
    {
		public virtual Type DataClass
		{
			get { return typeof(RoadwayData); }
		}

		public RoadwayData CreateDataObject()
		{
			return (RoadwayData)CreateInstance(DataClass);
		}
	}

	public interface IRoadwayMesh
	{
		bool Collidable { get; }

		Material Material { get; }

		public abstract Mesh GenerateMesh(CubicBezierCurve curve, RoadwayData data);
	}

	public interface IRoadwayObjects
	{
		public abstract List<GameObject> UpdateObjects(Segment segment, RoadwayData data, List<GameObject> gameObjects);
	}
}