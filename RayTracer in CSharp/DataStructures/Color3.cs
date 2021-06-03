using System;
using System.Collections.Generic;
using System.Text;

namespace Raytracing.DataStructures
{
    public struct Color3
    {
        #region Properties
        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }
        #endregion

        #region Functions
        public Color3(double r, double g, double b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }
        #endregion

        #region Operators
        public static Color3 operator *(double multiplier, Color3 color)
        {
            return new Color3(color.R * multiplier, color.G * multiplier, color.B * multiplier);
        }

        public static Color3 operator *(Color3 color, double multiplier)
        {
            return new Color3(color.R * multiplier, color.G * multiplier, color.B * multiplier);
        }

        public static Color3 operator +(Color3 color1, Color3 color2)
        {
            return new Color3(color1.R + color2.R, color1.G + color2.G, color1.B + color2.B);
        }
        #endregion

        #region Conversion
        public static implicit operator Vector3(Color3 c)
        {
            return new Vector3(c.R, c.G, c.B);
        }

        public static implicit operator Point3(Color3 c)
        {
            return new Point3(c.R, c.G, c.B);
        }

        public static implicit operator Color4(Color3 c)
        {
            return new Color4(c.R, c.G, c.B, 0);
        }

        public static implicit operator Color3(Vector3 v)
        {
            Vector3 unitVector = v.UnitVector;
            return new Color3(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));
        }
        #endregion
    }
}
