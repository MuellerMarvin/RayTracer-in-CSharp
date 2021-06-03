using System;
using System.Drawing.Imaging;
using Raytracing;
using Raytracing.DataStructures;
using Raytracing.Hittables;
using Raytracing.Materials;


namespace RayTracerConsole
{
    class Program
    {
        static void Main()
        {
            bool writeDebugInfo = false;

            IMaterial groundMaterial = new LambertianDiffuse(new Color3(0.8, 0.8, 0));
            IMaterial rightMaterial = new Metal(new Color3(0.8, 0.8, 0.8));
            IMaterial leftMaterial = new Dielectric(1.5);
            IMaterial centerMaterial = new LambertianDiffuse(new Color3(0.8,0.6,0.2));

            Renderer renderer = new()
            {
                // Define objects in the scene
                HittableObjects = new HittableList
                {
                    new Sphere(new Vector3(0, 1, -100.5), 100, groundMaterial),
                    new Sphere(new Vector3(0,1,0), 0.5, centerMaterial),
                    new Sphere(new Vector3(-1,1,0), 0.5, leftMaterial),
                    new Sphere(new Vector3(1, 1, 0), 0.5, rightMaterial),
                }
            };

            // Define the camera
            Camera camera = new(1280, 720)
            {
                Origin = new Vector3(0, 0, 0),
                MultithreadedRendering = true,
                SamplesPerPixel = 100,
                MaxBounces = 12
            };

            // Render
            RenderResult result = new();
            result.frameNumber = 0;
            result.camera = camera;
            result.pixels = renderer.RenderScene(camera, out result.frameTime);

            // Write to disk
            System.IO.Directory.CreateDirectory("./images/");
            Renderer.WriteFrame("./images/image_" + result.frameNumber + ".png", result.pixels, result.camera.ResolutionHeight, result.camera.ResolutionWidth, ImageFormat.Png, writeDebugInfo, result.frameTime, result.camera);
        }
    }

    struct RenderResult
    {
        public int frameNumber;
        public Camera camera;
        public Color4[] pixels;
        public long frameTime;
    }
}
