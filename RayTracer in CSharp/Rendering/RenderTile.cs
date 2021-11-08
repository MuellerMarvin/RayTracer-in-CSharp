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
        public Camera Camera { get; set; }
        public HittableList Hittables { get; set; }

        private static readonly ThreadLocal<Random> RanGen = new(() => new Random(Guid.NewGuid().GetHashCode()));

        public RenderTile(RenderSpace renderSpace, ref Camera camera, ref HittableList hittable)
        {
            this.RenderSpace = renderSpace;
            Pixels = new Color4[RenderSpace.PixelCount];
        }

        public static void Render(RenderTile tile)
        {
            // populate pixels
            tile.Pixels = new Color4[tile.RenderSpace.PixelCount];
            for (int i = 0; i < tile.Pixels.Length; i++)
            {
                tile.Pixels[i] = new Color4(0, 0, 0, 0);
            }

            for (int s = 0; s < tile.Camera.SamplesPerPixel; s++)
            {
                int p = 0;
                for (int x = tile.RenderSpace.StartX; x < tile.RenderSpace.EndX; x++)
                {
                    for (int y = tile.RenderSpace.StartY; y < tile.RenderSpace.EndY; y++)
                    {
                        tile.Pixels[p] += Renderer.GetRayColor(tile.Camera.GetRay(x + (RanGen.Value.NextDouble() * 2 - 1), y + (RanGen.Value.NextDouble() * 2 - 1)), tile.Hittables, tile.Camera.TransparentBackground, tile.Camera.MaxBounces);
                        p++;
                    }
                }
            }

            // divide by samples
            for (int i = 0; i < tile.Pixels.Length; i++)
            {
                tile.Pixels[i] = tile.Pixels[i] / tile.Camera.SamplesPerPixel;
            }
        }


        public static RenderTile[] CreateTiles(Camera camera, HittableList hittables)
        {
            List<RenderTile> renderTiles = new List<RenderTile>();

            // X-Points
            for (int x = 0; x < camera.Resolution.X; x += camera.TileWidth)
            {
                for (int y = 0; y < camera.Resolution.Y; y += camera.TileHeight)
                {
                    // cut off overlaps, if existent
                    int tileWidth = (x + camera.TileWidth) > camera.Resolution.X ? camera.Resolution.X : (x + camera.TileWidth);
                    int tileHeight = (y + camera.TileHeight) > camera.Resolution.Y ? camera.Resolution.Y : (y + camera.TileHeight);

                    //create tile
                    renderTiles.Add(new RenderTile(new RenderSpace(x, tileWidth, y, tileHeight), ref camera, ref hittables)); // something is really wrong here
                }
            }

            return renderTiles.ToArray();
        }

        public static Color4[] DrawTile(Color4[] pixels, Resolution resolution, RenderTile tile)
        {
            int i = 0;
            for (int x = tile.RenderSpace.StartX; x < tile.RenderSpace.EndX; x++)
            {
                for (int y = tile.RenderSpace.StartY; y < tile.RenderSpace.EndY; y++)
                {
                    pixels[x + y * resolution.X] = tile.Pixels[i];
                    i++;
                }
            }

            return pixels;
        }

        public static Color4[] DrawAllTiles(Color4[] pixels, Resolution resolution, RenderTile[] tiles)
        {
            foreach (RenderTile tile in tiles)
            {
                DrawTile(pixels, resolution, tile);
            }
            return pixels;
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
