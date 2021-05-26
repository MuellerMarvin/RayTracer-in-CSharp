using System;
using Raytracing;
using Raytracing.DataStructures;
using Raytracing.Hittables;

namespace PerformanceReporter
{
    class Program
    {
        static void Main(string[] args)
        {
            Raytracer raytracer = new Raytracer();

            int xRes = 720;
            int yRes = 480;

            int runs = 10;

            raytracer.HittableObjects = new HittableList()
            {
                new Sphere(0, 1, 0, 0.5),
                new Sphere(0.5, 1, 0, 0.5),
                new Sphere(0, 1, 0.6, 0.5),
                new Sphere(0, 1, -100.5, 100)
            };

            // Define the camera
            Camera camera = new Camera(xRes, yRes)
            {
                Origin = new Vector3(-1, 0, 0),
                MultithreadedRendering = true,
                SamplesPerPixel = 100,
                MaxBounces = 12
            };

            for(int i = 0; i < runs; i++)
            {
                raytracer.RenderScene(camera, out long frameTime);

                Console.WriteLine("Run {0} | Time: {1} | Time Per Pixel: {2}", i, frameTime, (double)frameTime / (double)(xRes * yRes));
            }
        }
    }
}
