using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Raytracing;
using Raytracing.DataStructures;
using Raytracing.Hittables;

namespace Raytracing.Rendering
{
    public class RenderTile
    {
        public RenderSpace RenderSpace { get; private set; }
        public Color4[] Pixels { get; set; }
        private static readonly ThreadLocal<Random> RanGen = new(() => new Random(Guid.NewGuid().GetHashCode()));

        public RenderTile(RenderSpace renderSpace)
        {
            this.RenderSpace = renderSpace;
            Pixels = new Color4[RenderSpace.PixelCount];
        }

        public void Render(HittableList hittables, Camera camera)
        {
            // populate pixels
            for (int i = 0; i < Pixels.Length; i++)
            {
                Pixels[i] = new Color4(0, 0, 0, 0);
            }

            for (int y = this.RenderSpace.StartY; y < this.RenderSpace.EndY; y++)
            {
                for (int x = this.RenderSpace.StartX; x < this.RenderSpace.EndX; x++)
                {
                    Pixels[x + y * this.RenderSpace.SpaceWidth] += Renderer.GetRayColor(camera.GetRay(x + (RanGen.Value.NextDouble() * 2 - 1), y + (RanGen.Value.NextDouble() * 2 - 1)), hittables, camera.TransparentBackground, camera.MaxBounces);
                }
            }

            // divide by samples
            for (int i = 0; i < camera.SamplesPerPixel; i++)
            {
                Pixels[i] = Pixels[i] / camera.SamplesPerPixel;
            }
        }
    }

    public struct RenderSpace
    {
        public int StartX { get; private set; }
        public int EndX { get; private set; }
        public int StartY { get; private set; }
        public int EndY { get; private set; }
        public int SpaceWidth { get; private set; }
        public int SpaceHeight { get; private set; }
        public int PixelCount { get; private set; }

        public RenderSpace(int startX, int endX, int startY, int endY)
        {
            this.StartX = startX;
            this.EndX = endX;
            this.StartY = startY;
            this.EndY = endY;
            this.SpaceWidth = (endX - startX);
            this.SpaceHeight = (endY - startY);
            this.PixelCount =  this.SpaceWidth * this.SpaceHeight;

        }
    }
}
