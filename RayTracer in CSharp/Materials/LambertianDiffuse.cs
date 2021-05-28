using Raytracing.DataStructures;

namespace Raytracing.Materials
{
    public class LambertianDiffuse : IMaterial
    {
        #region Properties
        public Color3 Albedo;
        #endregion

        #region Constructors
        public LambertianDiffuse()
        {
            Albedo = new Color3(0, 0, 0);
        }

        public LambertianDiffuse(Color3 albedo)
        {
            Albedo = albedo;
        }
        #endregion

        #region Functions
        public bool Scatter(Ray rayIn, HitRecord hitRecord, out Color3 colorAttenuation, out Ray scatteredRay)
        {
            // generate scattered diffuse ray
            scatteredRay = new Ray()
            {
                Origin = hitRecord.Point,
                Direction = hitRecord.Normal + Vector3.GetRandomUnitVector()
            };

            // catch degenerate scatter direction
            if (scatteredRay.Direction.isNearZero)
            {
                scatteredRay.Direction = hitRecord.Normal;
            }

            colorAttenuation = this.Albedo;
            
            // a diffuse material doesn't reflect, so it always scatters or absorbs (attenuation is the amount of absorbance)
            return true;
        }
        #endregion
    }
}   
