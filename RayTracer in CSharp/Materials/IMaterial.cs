using Raytracing.DataStructures;

namespace Raytracing.Materials
{
    public interface IMaterial
    {
        public bool Scatter(Ray rayIn, HitRecord hitRecord, Color3 colorAttenuation, Ray scatteredRay);
    }
}