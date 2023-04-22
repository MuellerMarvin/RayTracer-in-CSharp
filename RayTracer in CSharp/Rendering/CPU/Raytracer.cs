using System;
using Raytracing.Rendering;
using Raytracing.DataStructures;

namespace Raytracing.Rendering.CPU
{
	class Raytracer : IRaytracer
	{
		public bool Trace(Ray ray, out HitRecord hitRecord)
		{
            throw new NotImplementedException();
        }
        public bool TraceSphere(Vector3 center, float radius, Ray ray, out HitRecord hitRecord)
		{
            throw new NotImplementedException();
        }
	}
}