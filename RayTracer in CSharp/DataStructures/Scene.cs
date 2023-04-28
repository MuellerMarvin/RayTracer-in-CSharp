using Raytracing.Hittables;
using System.Collections.Generic;

namespace Raytracing.DataStructures
{
    public struct Scene
    {
        public HittableList Hittables;
        public Camera Camera;
    }
}