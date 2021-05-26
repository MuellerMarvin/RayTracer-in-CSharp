using Raytracing.DataStructures;

namespace Raytracing.Scene.Hittables
{
    /// <summary>
    /// An object that implements a hittable function, as well as an origin
    /// </summary>
    public interface IRenderObject : IHittable
    {
        public Point3 Origin { get; set; }
    }
}
