using System;
using Raytracing.DataStructures;

namespace Raytracing.Hittables
{
    public class Sphere : IHittable
    {
        #region Properties
        public Point3 Origin { get; set; }
        public double Radius { get; set; }
        public IMaterial Material { get; set; }
        #endregion

        #region Functions

        #region Constructors
        public Sphere(Point3 origin, double radius, IMaterial material)
        {
            this.Origin = (Point3)origin;
            this.Radius = radius;
            this.Material = material;
        }

        public Sphere(Vector3 origin, double radius, IMaterial material)
        {
            this.Origin = (Point3)origin;
            this.Radius = radius;
            this.Material = material;
        }

        public Sphere(double x, double y, double z, double radius, IMaterial material)
        {
            this.Origin = new Point3(x, y, z);
            this.Radius = radius;
            this.Material = material;
        }
        #endregion

        /// <summary>
        /// Check if the ray intersects with the sphere at any point
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        [Obsolete("This version of the function is not covered in the interface and should not be used.", false)]
        public bool Hit(Ray ray)
        {
            Vector3 oc = ray.Origin - (Vector3)this.Origin; // Ray Origin - Center of the Sphere = OC
            double a = ray.Direction.LengthSquared;
            double half_b = Vector3.Dot(oc, ray.Direction);
            double c = oc.LengthSquared - Radius * Radius;
            double discriminant = half_b * half_b - a * c;
            return (discriminant > 0);
        }

        /// <summary>
        /// Checks if the ray intersects with the sphere at a certain point
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="minDist"></param>
        /// <param name="maxDist"></param>
        /// <param name="hitRecord"></param>
        /// <returns></returns>
        public bool Hit(Ray ray, double minDist, double maxDist, out HitRecord hitRecord)
        {
            hitRecord = new HitRecord();
            hitRecord.Material = this.Material;

            Vector3 oc = ray.Origin - (Vector3)this.Origin; // Ray Origin - Center of the Sphere = OC
            double a = ray.Direction.LengthSquared;
            double half_b = Vector3.Dot(oc, ray.Direction);
            double c = oc.LengthSquared - Radius * Radius;
            double discriminant = half_b * half_b - a * c;

            if (discriminant > 0)
            {
                double root = Math.Sqrt(discriminant);
                double temp = (-half_b - root) / a;

                if (temp < maxDist && temp > minDist)
                {
                    hitRecord.Distance = temp;
                    hitRecord.Point = ray.PointAtDistance(hitRecord.Distance);
                    Vector3 outwardNormal = ((Vector3)hitRecord.Point - this.Origin) / this.Radius;

                    // determine if ray faces normal
                    hitRecord.HitFront = Vector3.Dot(ray.Direction, outwardNormal) < 0;
                    hitRecord.Normal = hitRecord.HitFront ? outwardNormal : -outwardNormal;

                    return true;
                }
                temp = (-half_b + root) / a;
                if (temp < maxDist && temp > minDist)
                {
                    hitRecord.Distance = temp;
                    hitRecord.Point = ray.PointAtDistance(hitRecord.Distance);
                    Vector3 outwardNormal = ((Vector3)hitRecord.Point - this.Origin) / this.Radius;

                    // determine if ray faces normal
                    hitRecord.HitFront = Vector3.Dot(ray.Direction, outwardNormal) < 0;
                    hitRecord.Normal = hitRecord.HitFront ? outwardNormal : -outwardNormal;
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
