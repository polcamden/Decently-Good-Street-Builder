using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace DecentlyGoodStreetBuilder
{
    [System.Serializable]
    public class MoveableHandle : ISelectable
    {
        [SerializeField] private Vector3 position;
        public Vector3 Position {
            get {
                return position;
            }
            set {
                position = value;
                if (callback != null)
                {
                    callback.Invoke();
                }
            }
        }
        
        const float size = 0.5f;

        [SerializeField] private UnityEvent callback;

        public void Init(Vector3 position, UnityEvent callback)
        {
            this.position = position;
            this.callback = callback;
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

            Debug.Log(cursorDistance);

            if (size > cursorDistance)
            {
                return new ISelectable[] { this };
            }

            return null;
        }
    }
}