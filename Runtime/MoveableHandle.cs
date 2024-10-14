using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace DecentlyGoodStreetBuilder
{
    [System.Serializable]
    public class MoveableHandle : ScriptableObject, ISelectable
    {
        [SerializeField] private Vector3 position;
        public Vector3 Position {
            get {
                return position;
            }
            set {
                Undo.RecordObject(this, "Handle Move");
                Undo.undoRedoEvent += OnPositionUndo;

                position = value;
                if (segment != null)
                {
                    segment.CalculateCurve();
                }
            }
        }
        
        const float size = 0.5f;

        [SerializeField] private Segment segment; //why do delgates not get serialized

        public MoveableHandle(Vector3 position, Segment segment)
        {
            this.position = position;
            this.segment = segment;
        }

        /// <summary>
        /// Draws a selectable handle
        /// </summary>
        public void Draw()
        {
            Handles.color = Color.blue;
            Handles.SphereHandleCap(0, position, Quaternion.identity, 0.5f, EventType.Repaint);
        }

        public ISelectable[] Selected()
        {
            float cursorDistance = HandleUtility.DistanceToCircle(Position, size);
            if (size > cursorDistance)
            {
                return new ISelectable[] { this };
            }

            return null;
        }

        public void OnPositionUndo(in UndoRedoInfo info)
        {
            segment.CalculateCurve();
        }
    }
}