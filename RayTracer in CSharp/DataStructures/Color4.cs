using System;
using System.Collections.Generic;
using System.Text;

namespace Raytracing.DataStructures
{
    public struct Color4
    {
        #region Properties
        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }
        public double A { get; set; }
        #endregion

        #region Functions
        public Color4(double r, double g, double b, double a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }
        #endregion

        #region Operators
        public static Color4 operator *(double multiplier, Color4 color)
        {
            return new Color4(color.R * multiplier, color.G * multiplier, color.B * multiplier, color.A * multiplier);
        }

        public static Color4 operator *(Color4 color, double multiplier)
        {
            return new Color4(color.R * multiplier, color.G * multiplier, color.B * multiplier, color.A * multiplier);
        }

        public static Color4 operator /(Color4 color, double divident)
        {
            return new Color4(color.R / divident, color.G / divident, color.B / divident, color.A / divident);
        }

        public static Color4 operator +(Color4 color1, Color4 color2)
        {
            return new Color4(color1.R + color2.R, color1.G + color2.G, color1.B + color2.B, color1.A + color2.A);
        }

        #endregion

        #region Conversion
        public static explicit operator Vector3(Color4 c)
        {
            return new Vector3(c.R, c.G, c.B);
        }

        public static explicit operator Point3(Color4 c)
        {
            return new Point3(c.R, c.G, c.B);
        }

        public static explicit operator Color3(Color4 c)
        {
            return new Color3(c.R, c.G, c.B);
        }
        #endregion
    }
}
