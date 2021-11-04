using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

using Raytracing.DataStructures;
using Raytracing.Hittables;

namespace Raytracing
{
    public class Renderer
    {
        #region Variables
        #region Properties
        public HittableList HittableObjects { get; set; }
        #endregion

        #region Internal Variables
        private static readonly ThreadLocal<Random> RanGen = new(() => new Random(Guid.NewGuid().GetHashCode()));
        #endregion
        #endregion

        #region Constructors
        public Renderer()
        {
            this.HittableObjects = new HittableList();
        }
        #endregion

        #region Functions
        #region RenderFunctions
        public Color4[] RenderScene(Camera camera)
        {
            #pragma warning disable IDE0059 // Unnecessary assignment of a value // prevents the error in user-code
            return RenderScene(camera, out long frametime);
            #pragma warning restore IDE0059 // Unnecessary assignment of a value
        }

        public Color4[] RenderScene(Camera camera, out long frameTime)
        {
            Stopwatch sw = new();
            Color4[] pixels;
            if (camera.MultithreadedRendering)
            {
                sw.Start();
                pixels = RenderSceneMultithreaded(camera);
                sw.Stop();
                frameTime = sw.ElapsedMilliseconds;
            }
            else
            {
                sw.Start();
                pixels = RenderSceneSingleThreaded(camera);
                sw.Stop();
                frameTime = sw.ElapsedMilliseconds;
            }
            return pixels;
        }

        private Color4[] RenderSceneSingleThreaded(Camera camera)
        {
            // set up buffer
            Color4[] frameBuffer = new Color4[camera.ResolutionHeight * camera.ResolutionWidth];

            // Create tasks that return the final color of a pixel
            for (int y = camera.ResolutionHeight - 1; y >= 0; --y)
            {
                for (int x = 0; x < camera.ResolutionWidth; ++x)
                {
                    frameBuffer[y * camera.ResolutionWidth + x] = RenderPixel(x, y, camera, this.HittableObjects);
                }
            }
            return frameBuffer;
        }

        private Color4[] RenderSceneMultithreaded(Camera camera)
        {
            // set up 
            Color4[] pixels = new Color4[camera.ResolutionHeight * camera.ResolutionWidth];
            Task<Color4>[] tasks = new Task<Color4>[camera.ResolutionHeight * camera.ResolutionWidth];

            // Create tasks that return the final color of a pixel
            for (int y = camera.ResolutionHeight - 1; y >= 0; --y)
            {
                for (int x = 0; x < camera.ResolutionWidth; ++x)
                {
                    var _x = x;
                    var _y = y;
                    tasks[y * camera.ResolutionWidth + x] = Task<Color4>.Run(() =>
                    {
                        return RenderPixel(_x, _y, camera, this.HittableObjects);
                    });
                }
            }
            Task.WaitAll(tasks);

            for (int i = 0; i < tasks.Length; i++)
            {
                pixels[i] = tasks[i].Result;
            }

            return pixels;
        }
        
        public Color4[] RenderDepthMap(double maxDist, Camera camera)
        {
            Color4[] depthMap = new Color4[camera.ResolutionHeight * camera.ResolutionWidth];

            for (int y = camera.ResolutionHeight - 1; y >= 0; --y)
            {
                for (int x = 0; x < camera.ResolutionWidth; ++x)
                {
                    // get the distance
                    double distance = GetDistanceOnPixel(x, y, camera);

                    // clamp it
                    distance = Math.Clamp(distance, 0, maxDist);

                    // scale it to fit the DepthMap (0 - 1)
                    int brightness = (int)Scale(distance, 0, maxDist, 0, 1);

                    depthMap[y * camera.ResolutionWidth + x] = new Color4(brightness, brightness, brightness, 1);
                }
            }

            return depthMap;
        }

        private double GetDistanceOnPixel(double x, double y, Camera camera)
        {
            if (HittableObjects.Hit(camera.GetRay(x, y), 0, double.PositiveInfinity, out HitRecord hitRecord))
            {
                return hitRecord.Distance;
            }
            else
            {
                return double.PositiveInfinity;
            }
        }

        public Color4[] RenderNormals(Camera camera)
        {
            Color4[] normals = new Color4[camera.ResolutionHeight * camera.ResolutionWidth];

            for (int y = camera.ResolutionHeight - 1; y >= 0; --y)
            {
                for (int x = 0; x < camera.ResolutionWidth; ++x)
                {

                    Color4 pixel = new(1, 1, 1, 1);

                    if(HittableObjects.Hit(camera.GetRay(x, y), 0, double.PositiveInfinity, out HitRecord hitRecord))
                    {
                        Vector3 vector = 0.5 * (hitRecord.Normal + new Vector3(1, 1, 1));
                        pixel = new Color4(vector.X, vector.Y, vector.Z, 1);
                    }

                    normals[y * camera.ResolutionWidth + x] = pixel;
                }
            }

            return normals;
        }


        static public Color4 RenderPixel(double x, double y, Camera camera, HittableList hittables)
        {
            Color4 pixelColor = new(0, 0, 0, 0);
            for (int i = 0; i < camera.SamplesPerPixel; i++)
            {
                pixelColor += RenderRay(camera.GetRay(x + (RanGen.Value.NextDouble() * 2 - 1), y + (RanGen.Value.NextDouble() * 2 - 1)), camera, hittables);
            }

            return pixelColor / camera.SamplesPerPixel;
        }

        static public Color4 RenderRay(Ray ray, Camera camera, HittableList hittables)
        {
            Color4 pixelColor = new(0, 0, 0, 0);
            for (int i = 0; i < camera.SamplesPerPixel; i++)
            {
                pixelColor += GetRayColor(ray, hittables, camera.TransparentBackground, camera.MaxBounces);
            }

            return pixelColor / camera.SamplesPerPixel;
        }

         static public Color4 GetRayColor(Ray ray, HittableList hittables, bool transparentBackground, int maxBounces)
        {
            if (maxBounces < 0)
                return new Color4(0, 0, 0, 1);

            Color4 pixelColor = new(0, 0, 0, 0);
            // check if an object is hit
            if (hittables.Hit(ray, 0.001, double.PositiveInfinity, out HitRecord hitRecord))
            {
                // check if this ray will scatter
                if (hitRecord.Material.Scatter(ray, hitRecord, out Color3 attenuation, out Ray scatteredRay))
                    return new Color4(attenuation, 1) * GetRayColor(scatteredRay, hittables, transparentBackground, maxBounces - 1);
            }
            // transparent background
            else if (!transparentBackground)
            {
                // draw a background
                double zPos = 0.5 * (ray.Direction.UnitVector.Z + 1);
                pixelColor = (1 - zPos) * new Color4(1, 1, 1, 1) + zPos * new Color4(0.5, 0.7, 1.0, 1);
                pixelColor.A = 1;
            }
            // else
            return pixelColor;
        }

        #endregion

        #region Write Functions
        public static void WriteFrame(string filePath, Color4[] pixels, int height, int width, ImageFormat format, bool writeDebugInfo, long frameTime, Camera camera)
        {
            Bitmap bitmap = ColorArrayToBitmap(width, height, pixels);

            if (writeDebugInfo)
            {
                string debugString = "Frametime: " + frameTime + " ms";
                debugString += " | Res: " + width + "x" + height + " | " + camera.SamplesPerPixel + " Samples/Pixel | " + camera.MaxBounces + " Max Bounces";

                Graphics g = Graphics.FromImage(bitmap);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(debugString, new Font("Tahoma", 8), Brushes.White, new PointF(5, height - 15));
            }

            bitmap.Save(filePath, format);
        }

        public static Bitmap ColorArrayToBitmap(int xRes, int yRes, Color4[] pixels)
        {
            Bitmap bitmap = new(xRes, yRes, PixelFormat.Format32bppArgb);
            for (int y = yRes - 1; y >= 0; y--)
            {
                for (int x = 0; x < xRes; x++)
                {
                    Color4 color4 = pixels[y * xRes + x];
                    bitmap.SetPixel(x, y, Color.FromArgb((int)(color4.A * 255), (int)(color4.R * 255), (int)(color4.G * 255), (int)(color4.B * 255)));
                }
            }
            return bitmap;
        }


        #endregion

        #region Utility

        private static double Scale(double value, double min, double max, double minScaled, double maxScaled)
        {
            double scaled = minScaled + (maxScaled - minScaled) * ((value - min) / (max - min));
            return scaled;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
        #endregion
        #endregion
    }
}
 