using Raytracing.DataStructures;

namespace Raytracing.Materials
{
    public interface IMaterial
    {
        /// <summary>
        /// Determines if the light will be scattered or reflected
        /// </summary>
        /// <param name="rayIn">The previous ray</param>
        /// <param name="hitRecord">Hitrecord of the previous raytrace</param>
        /// <param name="colorAttenuation">Factor of light absorbed</param>
        /// <param name="scatteredRay">The new directionn in which light will be scattered</param>
        /// <returns>A scattered ray, if the light ends up scattering</returns>
        public bool Scatter(Ray rayIn, HitRecord hitRecord, out Color3 colorAttenuation, out Ray scatteredRay);
    }
}