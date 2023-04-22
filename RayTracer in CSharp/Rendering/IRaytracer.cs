using System;
using Raytracing.DataStructures;

namespace Raytracing
{
    public interface IRaytracer
    {
        public bool Trace(Ray ray, out HitRecord hitRecord);
        public bool TraceSphere(Vector3 center, float radius, Ray ray, out HitRecord hitRecord);
    }
}