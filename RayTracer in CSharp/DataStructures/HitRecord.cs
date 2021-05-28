using Raytracing.Materials;

namespace Raytracing.DataStructures
{
    public struct HitRecord
    {
        /// <summary>
        /// Point of intersection
        /// </summary>
        public Point3 Point { get; set; }

        /// <summary>
        /// Surface-Normal at point of intersection
        /// </summary>
        public Vector3 Normal { get; set; }

        /// <summary>
        /// Distance from ray-origin
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// Determines if the surface was hit from its front
        /// </summary>
        public bool HitFront { get; set; }

        /// <summary>
        /// Determines how the surfacee interacts with rays
        /// </summary>
        public IMaterial Material { get; set; }
    }
}