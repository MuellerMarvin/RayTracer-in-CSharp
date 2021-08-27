using Raytracing.Hittables;
using Raytracing.DataStructures;

namespace Raytracing.SelfMesh
{
    public class Triangle : IHittable
    {
        public Point3[] Vertices { get; private set; }
        public Vector3 Normal { get; set; }

        #region Possibility of Editing
        public Point3 Vertex1 {
            get
            {
                return Vertices[0];
            }
            set
            {
                Vertices[0] = value;
            }
        }
        public Point3 Vertex2
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
        public Point3 Vertex3
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

            Vector3 v1 = (Vector3)this.Vertex1 + (Vector3)origin;
            Vector3 v2 = (Vector3)this.Vertex2 + (Vector3)origin;
            Vector3 v3 = (Vector3)this.Vertex3 + (Vector3)origin;

            Vector3 edge1 = new();
            Vector3 edge2 = new();

            Vector3 h = new();
            Vector3 s = new();
            Vector3 q = new();

            double a, f, u, v;

            edge1 = v2 - v1;
            edge2 = v3 - v1;

            h = Vector3.Cross(ray.Direction, edge2);

            a = Vector3.Dot(edge1, h);

            if (a > -Epsilon && a < Epsilon)
                return false; // ray is parallel to triangle

            f = 1.0d / a;

            s = (Vector3)ray.Origin - v1;
            u = f * (Vector3.Dot(s, h));

            if (u < 0 || u > 1)
                return false;

            q = Vector3.Cross(s, edge1);
            v = f * Vector3.Dot(ray.Direction, q);

            if (v < 0 || u + v > 1)
                return false;

            // find intersection point on ray with triangle
            double t = f * Vector3.Dot(edge2, q);
            if(t > Epsilon) // intersection
            {
                hitRecord.Normal = this.Normal;
                hitRecord.Distance = t;
                hitRecord.Material = new Materials.LambertianDiffuse(new Color3(0.8, 0.8, 0.8));
                hitRecord.Point = (t * ray.Direction) + ray.Origin;
                return true;
            }
            // else
            // no ray intersection
            return false;
        }

        private static readonly double Epsilon = 0.0000001d;
        #endregion
    }
}
