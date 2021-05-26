using System;
using System.Collections.Generic;
using System.Text;

namespace Raytracing.DataStructures
{
    public struct Ray
    {
        #region Properties
        public Point3 Origin { get; set; }
        public Vector3 Direction
        {
            get
            {
                return _Direction;
            }
            set
            {
                _Direction = value.UnitVector;
            }
        }
        private Vector3 _Direction;
        #endregion

        #region Functions
        /// <summary>
        /// Creates a new ray
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        public Ray(Point3 origin, Vector3 direction)
        {
            Origin = origin;
            _Direction = direction.UnitVector;
        }

        public Point3 PointAtDistance(double t)
        {
            return this.Origin + this.Direction * t;
        }
        #endregion
    }
}
