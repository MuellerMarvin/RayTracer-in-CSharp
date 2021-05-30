using Raytracing.DataStructures;

namespace Raytracing.Materials
{
    public class Metal : IMaterial
    {
        public Color3 Albedo { get; set; }

        public Metal(Color3 albedo)
        {
            this.Albedo = albedo;
        }

        public bool Scatter(Ray rayIn, HitRecord hitRecord, out Color3 colorAttenuation, out Ray scatteredRay)
        {
            // reflect the ray off the surface and make a new ray originating from the hit-point
            scatteredRay = new Ray(hitRecord.Point, Vector3.Reflect(rayIn.Direction.UnitVector, hitRecord.Normal));

            colorAttenuation = this.Albedo;

            return (Vector3.Dot(scatteredRay.Direction, hitRecord.Normal) > 0);
        }
    }
}