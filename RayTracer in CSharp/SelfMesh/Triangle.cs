using Raytracing.Hittables;
using Raytracing.DataStructures;

namespace Raytracing.SelfMesh
{
    public class Triangle : IHittable
    {
        public Point3[] Vertices { get; private set; }
        public Vector3 Normal { get; set; }

        #region Possibility of Editing
        public Point3 Vertice1 {
            get
            {
                return Vertices[0];
            }
            set
            {
                Vertices[0] = value;
            }
        }
        public Point3 Vertice2
        {
            get
            {
                return Vertices[1];
            }
            set
            {
                Vertices[1] = value;
            }
        }
        public Point3 Vertice3
        {
            get
            {
                return Vertices[2];
            }
            set
            {
                Vertices[2] = value;
            }
        }
        #endregion

        /// <summary>
        /// Create a new Triangle with all points , as well as the normal, at (0, 0, 0)
        /// </summary>
        public Triangle()
        {
            this.Vertices = new Point3[] { new Point3(0,0,0), new Point3(0, 0, 0), new Point3(0, 0, 0) };
            this.Normal = new Vector3(0,0,0);
        }

        /// <summary>
        /// Create a new Triangle with the given points
        /// </summary>
        public Triangle(Point3 point1, Point3 point2, Point3 point3, Vector3 normal)
        {
            this.Vertices = new Point3[] { point1, point2, point3 };
            this.Normal = normal;
        }

        #region Hit
        public bool Hit(Ray ray, double minDist, double maxDist, out HitRecord hitRecord)
        {
            return Hit(ray, minDist, maxDist, out hitRecord, new Point3(0, 0, 0));
        }

        public bool Hit(Ray ray, double minDist, double maxDist, out HitRecord hitRecord, Point3 origin)
        {
            hitRecord = new();
            return false;
        }
        #endregion
    }
}
