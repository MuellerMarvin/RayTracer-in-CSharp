using System;
using Raytracing.DataStructures;

namespace Raytracing.Scene.Hittables
{
    public class Sphere : IRenderObject
    {
        #region Properties
        public Point3 Origin { get; set; }
        public double Radius { get; set; }
        #endregion

        #region Functions

        #region Constructors
        public Sphere()
        {
            this.Origin = new Vector3(0, 0, 0);
            this.Radius = 1;
        }

        public Sphere(double radius)
        {
            this.Origin = new Vector3(0, 0, 0);
            this.Radius = radius;
        }

        public Sphere(double x, double y, double z)
        {
            this.Origin = new Point3(x, y, z);
            this.Radius = 1;
        }

        public Sphere(double x, double y, double z, double radius)
        {
            this.Origin = new Point3(x, y, z);
            this.Radius = radius;
        }
        #endregion

        /// <summary>
        /// Check if a ray hits the sphere
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public bool Hit(Ray ray)
        {
            Vector3 oc = ray.Origin - (Vector3)this.Origin; // Ray Origin - Center of the Sphere = OC
            double a = ray.Direction.LengthSquared;
            double half_b = Vector3.Dot(oc, ray.Direction);
            double c = oc.LengthSquared - Radius * Radius;
            double discriminant = half_b * half_b - a * c;
            return (discriminant > 0);
        }

        public bool Hit(Ray ray, double minDist, double maxDist, out HitRecord hitRecord)
        {
            hitRecord = new HitRecord();

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
