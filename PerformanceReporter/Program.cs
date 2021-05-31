using System;
using Raytracing;
using Raytracing.DataStructures;
using Raytracing.Hittables;
using Raytracing.Materials;

namespace PerformanceReporter
{
    class Program
    {
        static void Main(string[] args)
        {
            Renderer raytracer = new Renderer();

            int xRes = 720;
            int yRes = 480;

            int runs = 10;


            IMaterial cyan = new LambertianDiffuse(new Color3(0, 0.88, 0.88));
            IMaterial gray = new LambertianDiffuse(new Color3(0.5, 0.5, 0.5));
            IMaterial red = new LambertianDiffuse(new Color3(0.8, 0.2, 0.2));
            IMaterial metal = new Metal(new Color3(0.9, 0.9, 0.9), 0);

            Renderer renderer = new()
            {
                // Define objects in the scene
                HittableObjects = new HittableList
                {
                    new Sphere(0, 1, 0, 0.5, gray),
                    new Sphere(1, 1, 0, 0.5, red),
                    new Sphere(0, 1, -100.5, 100, metal),
                    new Sphere(0, -2, 0, 1, cyan)
                }
            };

            // Define the camera
            Camera camera = new(xRes, yRes)
            {
                Origin = new Vector3(-1, 0, 0),
                MultithreadedRendering = true,
                SamplesPerPixel = 100,
                MaxBounces = 12
            };

            for (int i = 0; i < runs; i++)
            {
                raytracer.RenderScene(camera, out long frameTime);

                Console.WriteLine("Run {0} | Time: {1} | Time Per Pixel: {2}", i, frameTime, (double)frameTime / (double)(xRes * yRes));
            }
        }
    }
}
