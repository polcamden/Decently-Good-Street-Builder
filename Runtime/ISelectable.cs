using UnityEngine;

namespace DecentlyGoodStreetBuilder
{
    public interface ISelectable
    {
        public Vector3 Position { get; set; }

        /// <summary>
        /// returns if this or other ISelectables have been selected
        /// </summary>
        /// <returns></returns>
        public ISelectable[] Selected();
    }
}