﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Raytracing;
using Raytracing.DataStructures;
using Raytracing.Hittables;
using Raytracing.Materials;
using Raytracing.SelfMesh;

namespace Raytracing.Rendering
{
    public class RenderEngine
    {
        Camera Camera { get; set; }
        HittableList Hittables { get; set; }

        public RenderEngine(Camera camera, HittableList hittables)
        {
            this.Camera = camera;
            this.Hittables = hittables;
        }
    }
}
