using Raytracing.DataStructures;

namespace Raytracing.Materials
{
    public class Metal : IMaterial
    {
        public Color3 Albedo { get; set; }
        public double Roughness
        {
            get
            {
                return _Roughness;
            }
            set
            {
                if(value > 1)
                {
                    _Roughness = 1;
                }
                else
                {
                    _Roughness = value;
                }
            }
        }
        private double _Roughness = 0;

        public Metal(Color3 albedo)
        {
            this.Albedo = albedo;
        }

        public Metal(Color3 albedo, double roughness)
        {
            this.Albedo = albedo;
            this.Roughness = roughness;
        }

        public bool Scatter(Ray rayIn, HitRecord hitRecord, out Color3 colorAttenuation, out Ray scatteredRay)
        {
            // get roughness, save cpu-time by ignoring it when there is none
            
            Vector3 roughnessVector = this.Roughness * Vector3.GetRandomInUnitSphere();

            // reflect the ray off the surface and make a new ray originating from the hit-point
            scatteredRay = new Ray(hitRecord.Point, Vector3.Reflect(rayIn.Direction.UnitVector, hitRecord.Normal) + roughnessVector);

            colorAttenuation = this.Albedo;

            return (Vector3.Dot(scatteredRay.Direction, hitRecord.Normal) > 0);
        }
    }
}