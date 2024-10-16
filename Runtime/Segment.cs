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

            handle = new MoveableHandle[] { new MoveableHandle(Vector3.zero, this), new MoveableHandle(Vector3.zero, this) };

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
            if (curveType != SegmentCurveType.Free)
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
            ISelectable[] result = handle[0].Selected();
            result = handle[1].Selected();

            if(result != null)
            {
                return result;
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
            Vector3 newPos = Vector3.Lerp(connection[0].Position, connection[1].Position, 0.5f);
            Vector3 positionDifference = newPos - Position;
            base.Position = newPos;
            
            UpdateHandle(positionDifference);
            CalculateCurve();
        }

        /// <summary>
        /// Calculates the curve for drawing
        /// </summary>
        public void CalculateCurve()
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
            switch (curveType)
            {
                case SegmentCurveType.Straight:
                    StraightCurve();
                    break;
                case SegmentCurveType.Curve:
                    SmoothCurveHandle();
                    break;
                case SegmentCurveType.Free:
                    //HandleWorldPosition += positionDifference;
                    break;
            }
        }

        private void StraightCurve()
        {
            Vector3 h1 = Vector3.Lerp(connection[0].Position, connection[1].Position, 1/3f);
            Vector3 h2 = Vector3.Lerp(connection[0].Position, connection[1].Position, 2/3f);

            SetHandleWorldPosition(0, h1);
            SetHandleWorldPosition(1, h2);
        }

        private void SmoothCurveHandle()
        {
            float curveDistance = 0;
            for (int i = 1; i < curve.Length; i++)
            {
                curveDistance += Vector3.Distance(curve[i - 1], curve[i]);
            }

            if (connection[0].ConnectionCount == 2)
            {
                Node n = connection[0].GetConnection(0);
                if(n == connection[1])
                {
                    n = connection[0].GetConnection(1);
                }

                Vector3 handleNormal = (connection[1].Position - n.Position).normalized;

                Debug.Log((n.Position - connection[1].Position).normalized);
                SetHandleWorldPosition(0, handleNormal * (curveDistance / 3) + connection[0].Position);
            }
            else
            {
                Vector3 h1 = Vector3.Lerp(connection[0].Position, connection[1].Position, 1 / 3f);
                SetHandleWorldPosition(0, h1);
            }

            if (connection[1].ConnectionCount == 2)
            {
                Node n = connection[1].GetConnection(0);
                if (n == connection[0])
                {
                    n = connection[1].GetConnection(1);
                }

                Vector3 handleNormal = (connection[0].Position - n.Position).normalized;
                SetHandleWorldPosition(1, handleNormal * (curveDistance / 3) + connection[0].Position);
            }
            else
            {
                Vector3 h1 = Vector3.Lerp(connection[0].Position, connection[1].Position, 2 / 3f);
                SetHandleWorldPosition(1, h1);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (connection[0] != null && connection[1] != null)
            {
                connection[0].RemoveConnection(this);
                connection[1].RemoveConnection(this);
            }
        }
    }

    public enum SegmentCurveType
    {
        Straight,
        Curve,
        Free
    }
}