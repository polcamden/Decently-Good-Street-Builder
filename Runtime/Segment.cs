using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace DecentlyGoodStreetBuilder
{
    public class Segment : StreetElement
    {
        public override Vector3 Position { 
            get => base.Position;
            set {
                
                Vector3 add = value - base.Position;
                connection[0].Position += add;
                connection[1].Position += add;

                //Undo.RecordObjects(connection, "Move");
                /*
                Vector3 add = base.Position - value;

                connection[0].Position += add;
                connection[1].Position += add;
                base.Position = value;*/
            } 
        }

        [SerializeField] private Node[] connection;

        //curve handle
        [SerializeField] private MoveableHandle[] handle;
        [SerializeField] private SegmentCurveType curveType = SegmentCurveType.Curve;
        public SegmentCurveType CurveType
        {
            get
            {
                return curveType;
            }
            set
            {
                curveType = value;
                OnPositionChange();
            }
        }

        [SerializeField] private Vector3[] endPoints;
        public Vector3 endPointsWorldPosition(int i)
        {
            return Position + endPoints[i];
        }

        public Vector3 GetHandleWorldPosition(int i)
        {
            return handle[i].Position;
        }

        public void SetHandleWorldPosition(int i, Vector3 value)
        {
            handle[i].Position = value;
        }

        [SerializeField] private Vector3[] curve;

        //consts
        const float SELECTION_DISTANCE = 10;

        /// <summary>
        /// Sets Group and links the segment to two nodes
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <param name="streetBuilder"></param>
        /// <param name="elementGroup"></param>
        public void Init(Node node1, Node node2, StreetBuilder streetBuilder, ElementGroup elementGroup = null)
        {
            base.Init(streetBuilder, elementGroup);

            //error check make sure node1 isnt already connected 
            //call add connection in both nodes
            bool prevConFound = false;
            for (int i = 0; i < node1.ConnectionCount; i++) { 
                Node n = node1.GetConnection(i);

                if (n == node2)
                {
                    prevConFound = true;
                }
            }

            if (prevConFound)
            {
                DestroyImmediate(this);
                return;
            }

            connection = new Node[2];
            connection[0] = node1;
            connection[1] = node2;

            endPoints = new Vector3[2];

            node1.AddConnection(node2, this);
            node2.AddConnection(node1, this);

            handle = new MoveableHandle[] { CreateInstance<MoveableHandle>(), CreateInstance<MoveableHandle>() };
            handle[0].Init(Vector3.zero, this);
            handle[1].Init(Vector3.zero, this);

            OnPositionChange();
        }

        /// <summary>
        /// Should not be use
        /// </summary>
        /// <param name="streetBuilder"></param>
        /// <param name="elementGroup"></param>
        public override void Init(StreetBuilder streetBuilder, ElementGroup elementGroup = null)
        {
            Debug.LogWarning("do not use base Init for segments");
            DestroyImmediate(this);
        }

        public override void Draw(string[] args)
        {
            if (curveType == SegmentCurveType.Free)
            {
                handle[0].Draw();
                handle[1].Draw();
            }

            Handles.color = Color.black;
            if (args.Contains<string>("selected"))
            {
                Handles.color = Color.red;
            }

            if(curve != null && curve.Length >= 2)
            {
                for (int i = 0; i < curve.Length-1; i++)
                {
                    Handles.DrawLine(curve[i], curve[i + 1], 1);
                }
            }
        }

        public override ISelectable[] Selected()
        {
            if (curveType == SegmentCurveType.Free)
            {
                for(int i = 0;i < 2; i++)
                {
                    ISelectable[] result = handle[i].Selected();

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            if (curve != null && curve.Length >= 2)
            {
                for (int i = 0; i < curve.Length - 1; i++)
                {
                    float cursorDistance = HandleUtility.DistanceToLine(curve[i], curve[i+1]);

                    if (SELECTION_DISTANCE > cursorDistance)
                    {
                        return new StreetElement[] { this };
                    }
                }
            }

            return null;
        }

        public override void OnPositionChange()
        {
            base.OnPositionChange();

            Vector3 newPos = Vector3.Lerp(connection[0].Position, connection[1].Position, 0.5f);
            Vector3 positionDifference = newPos - Position;
            base.Position = newPos;
            
            UpdateHandle(positionDifference);
            CalculateCurve();

            //update neighboring segments
            UpdateAdjacents();
		}

        private void UpdateAdjacents()
        {
			for (int i = 0; i < 2; i++)
			{
				Node n = connection[i];

				for (int j = 0; j < n.ConnectionCount; j++)
				{
					Segment seg = n.GetConnectionLink(j);

					if (seg != this)
					{
						n.GetConnectionLink(j).AdjacentSegmentHasUpdated();
					}
				}
			}
		}

		/// <summary>
		/// Called by adjacent segment to update the handles
		/// </summary>
		public void AdjacentSegmentHasUpdated()
        {
			UpdateHandle(Vector3.zero);
			CalculateCurve();
		}

        /// <summary>
        /// Calculates the curve for drawing
        /// </summary>
        private void CalculateCurve()
        {
            Vector3 a1 = connection[0].Position;
            Vector3 a2 = connection[1].Position;
            Vector3 h1 = GetHandleWorldPosition(0);
            Vector3 h2 = GetHandleWorldPosition(1);

            curve = GeometryF.CubicBezierCurvePoints(a1, a2, h1, h2, 1f);
        }

        /// <summary>
        /// Updates handle position based on the curve type and position difference
        /// </summary>
        /// <param name="positionDifference"></param>
        private void UpdateHandle(Vector3 positionDifference)
        {
            float curveDistance = 0;
            if (curve != null && curve.Length >= 2)
            {
                for (int i = 1; i < curve.Length; i++)
                {
                    curveDistance += Vector3.Distance(curve[i - 1], curve[i]);
                }
            }

            for (int i = 0; i < 2; i++)
            {
                int oppositeNode = i == 0 ? 1 : 0;

                if (curveType == SegmentCurveType.Curve && connection[i].ConnectionCount == 2)
                {
                    Node n = connection[i].GetConnection(0);
                    if (n == connection[oppositeNode])
                    {
                        n = connection[i].GetConnection(1);
                    }

                    Vector3 handleNormal = (connection[oppositeNode].Position - n.Position).normalized;
                    SetHandleWorldPosition(i, handleNormal * (curveDistance / 3) + connection[i].Position);
                }
                else if (curveType == SegmentCurveType.Straight || curveType == SegmentCurveType.Curve)
                {
                    Vector3 h1 = Vector3.Lerp(connection[0].Position, connection[1].Position, (i+1) / 3f);
                    SetHandleWorldPosition(i, h1);
                }
            }
        }

        public override void OnDestroy()
        {
			base.OnDestroy();

			if (connection != null && connection[0] != null && connection[1] != null)
            {
                connection[0].RemoveConnection(this);
                connection[1].RemoveConnection(this);
            }

			UpdateAdjacents();
		}
    }

    public enum SegmentCurveType
    {
        Straight,
        Curve,
        Free
    }
}