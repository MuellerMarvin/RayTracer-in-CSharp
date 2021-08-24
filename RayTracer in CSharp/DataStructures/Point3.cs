using System;
using System.Collections.Generic;
using System.Text;

namespace Raytracing.DataStructures
{
    public struct Point3
    {
        #region Properties
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public static Point3 Zero { get { return new Point3(0, 0, 0); } }
        #endregion

        #region Functions
        public Point3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        #endregion

        #region Conversion
        #region Interclass-casting
        public static implicit operator Color3(Point3 p)
        {
            return new Color3(p.X, p.Y, p.Z);
        }

        public static implicit operator Vector3(Point3 p)
        {
            return new Vector3(p.X, p.Y, p.Z);
        }
        #endregion
        #endregion
    }
}
