using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Raytracing.DataStructures
{
    public struct Vector3
    {
        #region Static Values
        //private static readonly Random RandomGen = new Random();
        public static readonly ThreadLocal<Random> RandomGen = new(() => new Random(Guid.NewGuid().GetHashCode()));

        public static Vector3 Zero
        {
            get
            {
                return new Vector3(0, 0, 0);
            }
        }
        #endregion

        #region Properties
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        /// <summary>
        /// Returns true if all axis are < 0.00000001
        /// </summary>
        public bool IsNearZero
        {
            get
            {
                const double s = 1e-8; // 0.00000001
                return (this.X < s) && (this.Y < s) && (this.Z < s);
            }
        }

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
        public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(vector1.Y * vector2.Z - vector1.Z * vector2.Y,
                               vector1.Z * vector2.X - vector1.X * vector2.Z,
                               vector1.X * vector2.Y - vector1.Y * vector2.X);
        }

        /// <summary>
        /// Reflects the input ray from a surface with a specific normal
        /// </summary>
        /// <returns></returns>
        public static Vector3 Reflect(Vector3 vector, Vector3 normal)
        {
            return vector - 2 * Vector3.Dot(vector, normal) * normal;
        }

        #region Randomness
        public static Vector3 GetRandomVector()
        {
            return new Vector3(RandomGen.Value.NextDouble() * 2 - 1, RandomGen.Value.NextDouble() * 2 - 1, RandomGen.Value.NextDouble() * 2 - 1);
        }

        public static Vector3 GetRandomUnitVector()
        {
            while (true)
            {
                Vector3 vector = new(RandomGen.Value.NextDouble(), RandomGen.Value.NextDouble(), RandomGen.Value.NextDouble());
                if (vector.LengthSquared >= 1) continue; // continue if not in the sphere
                return vector; // else
            }
        }

        public static Vector3 GetRandomInUnitSphere()
        {
            while (true)
            {
                // create random vector
                Vector3 vector = GetRandomVector();

                // if it's length isn't on the sphere, reject it
                if (vector.LengthSquared >= 1)
                    continue;
                // else return it
                return vector;
            }
        }
        #endregion

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
        public static implicit operator Point3(Vector3 v)
        {
            return new Point3(v.X, v.Y, v.Z);
        }
        #endregion
        #endregion
    }
}
