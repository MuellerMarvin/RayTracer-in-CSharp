using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Raytracing.DataStructures;
using Raytracing.Hittables;

namespace Raytracing.Rendering
{
    public class RenderingFunctions
    {
        // ThreadLocal random generator
        private static readonly ThreadLocal<Random> RanGen = new(() => new Random(Guid.NewGuid().GetHashCode()));

        public static Color4[] RenderSceneRenderScene(Camera camera, HittableList hittables)
        {
            #pragma warning disable IDE0059 // Unnecessary assignment of a value // prevents the error in user-code
            return RenderScene(camera, hittables, out long frametime);
            #pragma warning restore IDE0059 // Unnecessary assignment of a value
        }

        public static Color4[] RenderScene(Camera camera, HittableList hittables, out long frameTime)
        {
            Stopwatch sw = new();
            Color4[] pixels;
            if (camera.MultithreadedRendering)
            {
                sw.Start();
                pixels = RenderSceneMultithreaded(camera, hittables);
                sw.Stop();
                frameTime = sw.ElapsedMilliseconds;
            }
            else
            {
                sw.Start();
                pixels = RenderSceneSingleThreaded(camera, hittables);
                sw.Stop();
                frameTime = sw.ElapsedMilliseconds;
            }
            return pixels;
        }

        private static Color4[] RenderSceneSingleThreaded(Camera camera, HittableList hittableObjects)
        {
            // set up buffer
            Color4[] frameBuffer = new Color4[camera.Resolution.Y * camera.Resolution.X];

            // Create tasks that return the final color of a pixel
            for (int y = camera.Resolution.Y - 1; y >= 0; --y)
            {
                for (int x = 0; x < camera.Resolution.X; ++x)
                {
                    frameBuffer[y * camera.Resolution.X + x] = RenderPixel(x, y, camera, hittableObjects);
                }
            }
            return frameBuffer;
        }

        private static Color4[] RenderSceneMultithreaded(Camera camera, HittableList hittables)
        {
            // set up 
            Color4[] pixels = new Color4[camera.Resolution.Y * camera.Resolution.X];
            Task<Color4>[] tasks = new Task<Color4>[camera.Resolution.Y * camera.Resolution.X];

            // Create tasks that return the final color of a pixel
            for (int y = camera.Resolution.Y - 1; y >= 0; --y)
            {
                for (int x = 0; x < camera.Resolution.X; ++x)
                {
                    var _x = x;
                    var _y = y;
                    tasks[y * camera.Resolution.X + x] = Task<Color4>.Run(() =>
                    {
                        return RenderPixel(_x, _y, camera, hittables);
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

        public static Color4[] RenderDepthMap(double maxDist, Camera camera, HittableList hittables)
        {
            Color4[] depthMap = new Color4[camera.Resolution.Y * camera.Resolution.X];

            for (int y = camera.Resolution.Y - 1; y >= 0; --y)
            {
                for (int x = 0; x < camera.Resolution.X; ++x)
                {
                    // get the distance
                    double distance = GetDistanceOnPixel(x, y, camera, hittables);

                    // clamp it
                    distance = Math.Clamp(distance, 0, maxDist);

                    // scale it to fit the DepthMap (0 - 1)
                    int brightness = (int)Raytracing.Maths.MathHelpers.Scale(distance, 0, maxDist, 0, 1);

                    depthMap[y * camera.Resolution.X + x] = new Color4(brightness, brightness, brightness, 1);
                }
            }

            return depthMap;
        }

        public static double GetDistanceOnPixel(double x, double y, Camera camera, HittableList hittables)
        {
            if (hittables.Hit(camera.GetRay(x, y), 0, double.PositiveInfinity, out HitRecord hitRecord))
            {
                return hitRecord.Distance;
            }
            else
            {
                return double.PositiveInfinity;
            }
        }

        public Color4[] RenderNormals(Camera camera, HittableList hittables)
        {
            Color4[] normals = new Color4[camera.Resolution.Y * camera.Resolution.X];

            for (int y = camera.Resolution.Y - 1; y >= 0; --y)
            {
                for (int x = 0; x < camera.Resolution.X; ++x)
                {

                    Color4 pixel = new(1, 1, 1, 1);

                    if (hittables.Hit(camera.GetRay(x, y), 0, double.PositiveInfinity, out HitRecord hitRecord))
                    {
                        Vector3 vector = 0.5 * (hitRecord.Normal + new Vector3(1, 1, 1));
                        pixel = new Color4(vector.X, vector.Y, vector.Z, 1);
                    }

                    normals[y * camera.Resolution.X + x] = pixel;
                }
            }

            return normals;
        }


        public static Color4 RenderPixel(double x, double y, Camera camera, HittableList hittables)
        {
            Color4 pixelColor = new(0, 0, 0, 0);
            for (int i = 0; i < camera.SamplesPerPixel; i++)
            {
                pixelColor += GetRayColor(camera.GetRay(x + (RanGen.Value.NextDouble() * 2 - 1), y + (RanGen.Value.NextDouble() * 2 - 1)), hittables, camera.TransparentBackground, camera.MaxBounces);
            }

            return pixelColor / camera.SamplesPerPixel;
        }

        public static Color4 GetRayColor(Ray ray, HittableList hittables, bool transparentBackground, int maxBounces)
        {
            if (maxBounces < 0)
                return new Color4(0, 0, 0, 1);

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
                Color4 pixelColor = (1 - zPos) * new Color4(1, 1, 1, 1) + zPos * new Color4(0.5, 0.7, 1.0, 1);
                pixelColor.A = 1;
                return pixelColor;
            }
            // return transparent backgrouns
            return new Color4(0, 0, 0, 0);
        }
    }
}