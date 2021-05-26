using Raytracing.DataStructures;

namespace Raytracing.Hittables
{
    /// <summary>
    /// An object that implements a hittable function
    /// </summary>
    public interface IHittable
    {
        public bool Hit(Ray ray, double minDist, double maxDist, out HitRecord hitRecord);
    }
}
