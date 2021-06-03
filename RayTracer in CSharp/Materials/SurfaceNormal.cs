using Raytracing.DataStructures;

namespace Raytracing.Materials
{
    public class SurfaceNormal : IMaterial
    {
        /// <summary>
        /// This material needs to borrow a scattering behaviour from another one.
        /// </summary>
        IMaterial SubMaterial { get; set; }

        /// <summary>
        /// Makes a new surface normal material, borrowing the scattering behaviour from another one
        /// </summary>
        /// <param name="subMaterial"></param>
        public SurfaceNormal(IMaterial subMaterial)
        {
            this.SubMaterial = subMaterial;
        }

        public bool Scatter(Ray rayIn, HitRecord hitRecord, out Color3 colorAttenuation, out Ray scatteredRay)
        {
            colorAttenuation = hitRecord.Normal.UnitVector;

            Color3 strawman;
            bool scatter = this.SubMaterial.Scatter(rayIn, hitRecord, out strawman, out scatteredRay);
            return scatter;
        }
    }
}
