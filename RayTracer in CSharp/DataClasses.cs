using System;

namespace DataClasses
{
    public class Point3
    {
        #region Properties
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
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

    public class Vector3
    {
        #region Properties
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new 3-Dimensional Vector
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        #endregion

        #region Operators
        /// <summary>
        /// Just the itself vector with a plus in front of it should not change anything
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 operator +(Vector3 v) => v;

        /// <summary>
        /// Adds 2 Vectors together
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static Vector3 operator +(Vector3 vector1, Vector3 vector2)
        {
            vector1.X += vector2.X;
            vector1.Y += vector2.Y;
            vector1.Z += vector2.Z;
            return vector1;
        }

        /// <summary>
        /// A minus in front of the vector negates it
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3(-v.X, -v.Y, -v.Z);
        }
        /// <summary>
        /// Subtracts two vectors from each other
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static Vector3 operator -(Vector3 vector1, Vector3 vector2)
        {
            vector1.X -= vector2.X;
            vector1.Y -= vector2.Y;
            vector1.Z -= vector2.Z;
            return vector1;
        }

        /// <summary>
        /// Multiplies all values of the vector
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static Vector3 operator *(double multiplier, Vector3 vector)
        {
            return new Vector3(vector.X * multiplier, vector.Y * multiplier, vector.Z * multiplier);
        }


        /// <summary>
        /// Multiplies all values of the vector
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static Vector3 operator *(Vector3 vector, double multiplier)
        {
            return new Vector3(vector.X * multiplier, vector.Y * multiplier, vector.Z * multiplier);
        }

        /// <summary>
        /// Multiplies 2 vectors with each other
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector3 operator *(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }

        /// <summary>
        /// Divides all values of the vector by a value
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="divident"></param>
        /// <returns></returns>
        public static Vector3 operator /(Vector3 vector, double divident)
        {
            return vector * (1 / divident);
        }

        /// <summary>
        /// Makes the vector behave like a 3-item-long double-array
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double this[int key]
        {
            get
            {
                return this.ToArray()[key];
            }
            set
            {
                double[] array = this.ToArray();
                array[key] = value;
                this.FromArray(array);
            }
        }
        #endregion

        #region Utility

        /// <summary>
        /// Gets the length of the vector from 0, 0, 0
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt(this.LengthSquared);
            }
        }

        public double LengthSquared
        {
            get
            {
                return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
            }
        }

        /// <summary>
        /// Calculate "unit vector" (Vector with the length of 1)
        /// </summary>
        public Vector3 UnitVector => (this / this.Length);


        /// <summary>
        /// Dots two different Vectors with one another
        /// </summary>
        /// <param name="vector1">First vector to dot</param>
        /// <param name="vector2">Second vector to dot with</param>
        /// <returns></returns>
        public static double Dot(Vector3 vector1, Vector3 vector2)
        {
            return vector1.X * vector2.X
                 + vector1.Y * vector2.Y
                 + vector1.Z * vector2.Z;
        }

        /// <summary>
        /// Crosses two vectors
        /// </summary>
        /// <param name="targetVector"></param>
        /// <returns></returns>
        public Vector3 Cross(Vector3 targetVector)
        {
            return new Vector3(this.Y * targetVector.Z - this.Z * targetVector.Y,
                               this.Z * targetVector.X - this.X * targetVector.Z,
                               this.X * targetVector.Y - this.Y * targetVector.X);
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Converts this object's data and turns it into a double-array
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            return new double[] { this.X, this.Y, this.Z };
        }

        /// <summary>
        /// Takes data from a double-array and converts it to a Vector3 object
        /// </summary>
        /// <param name="array"></param>
        public void FromArray(double[] array)
        {
            this.X = array[0];
            this.Y = array[1];
            this.Z = array[2];
        }
        #region Interclass-casting
        public static implicit operator Color3(Vector3 v)
        {
            return new Color3(v.X, v.Y, v.Z);
        }

        public static implicit operator Point3(Vector3 v)
        {
            return new Point3(v.X, v.Y, v.Z);
        }
        #endregion
        #endregion
    }

    public class Color3
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
        #endregion
    }

    public class Color4
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

    public class Ray
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
            this.Origin = origin;
            this.Direction = direction;
        }

        public Point3 PointAtDistance(double t)
        {
            return this.Origin + this.Direction * t;
        }
        #endregion
    }
}
