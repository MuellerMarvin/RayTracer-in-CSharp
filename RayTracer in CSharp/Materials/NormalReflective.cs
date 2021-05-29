using Raytracing.DataStructures;

namespace Raytracing.Materials
{
    public class NormalReflective : IMaterial
    {
        public Color3 Albedo;

        public NormalReflective(Color3 albedo)
        {
            this.Albedo = albedo;
        }

        public bool Scatter(Ray rayIn, HitRecord hitRecord, out Color3 colorAttenuation, out Ray scatteredRay)
        {
            scatteredRay = new Ray(hitRecord.Point, hitRecord.Normal);
            colorAttenuation = this.Albedo;
            return true;
        }
    }
}
