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

            IMaterial cyan = new LambertianDiffuse(new Color3(0, 0.88, 0.88));
            IMaterial gray = new LambertianDiffuse(new Color3(0.8, 0.8, 0.8));
            IMaterial reddish = new LambertianDiffuse(new Color3(1, 0.5, 0.39));
            IMaterial purple = new LambertianDiffuse(new Color3(0.7, 0.1, 0.8));
            IMaterial metal = new Metal(new Color3(0.9, 0.9, 0.9), 0.01);

            Renderer renderer = new()
            {
                // Define objects in the scene
                HittableObjects = new HittableList
                {
                    new Sphere(-1, 1, 0, 0.5, purple),
                    new Sphere(0, 1, 0, 0.5, metal),
                    new Sphere(1, 1, 0, 0.5, reddish),
                    new Sphere(0, 1, -100.5, 100, gray),
                    new Sphere(0, -2, 0, 1, cyan)
                }
            };

            // Define the camera
            Camera camera = new(720, 480)
            {
                Origin = new Vector3(-1, 0, 0),
                MultithreadedRendering = true,
                SamplesPerPixel = 100,
                MaxBounces = 12
            };

            Console.WriteLine("Rendering...");
            const int frames = 200;
            long lastFrameTime = 0;
            for (int i = 0; i < frames; i++)
            {
                // calculate time remaining
                string timeRemaining = String.Empty;
                if (lastFrameTime != 0)
                {
                    timeRemaining = "Time remaining: ~" + Math.Round(((double)lastFrameTime / 1000d / 60d) * frames).ToString() + " minutes";
                }

                // provide a status
                Console.CursorLeft = 0;
                Console.CursorTop = 1;
                int percentage = (int)Math.Round((float)(i) / ((float)frames / 100f));
                Console.Write("{0} out of {1} completed. ({2} %) {3}\n{4}", i, frames, percentage, timeRemaining, GetProgressBar(50, percentage));

                // define camera + changes to it
                Camera localCam = new(camera.ResolutionWidth, camera.ResolutionHeight)
                {
                    Origin = camera.Origin + new Vector3(0.01f * i, 0, 0),
                    MultithreadedRendering = camera.MultithreadedRendering,
                    SamplesPerPixel = camera.SamplesPerPixel,
                    MaxBounces = camera.MaxBounces
                };

                RenderResult result = new()
                {
                    frameNumber = i,
                    camera = localCam
                };

                // Render
                result.pixels = renderer.RenderScene(localCam, out long frameTime);
                result.frameTime = frameTime;

                // Write to disk
                System.IO.Directory.CreateDirectory("./images/");
                renderer.WriteFrame("./images/image_" + result.frameNumber + ".png", result.pixels, result.camera.ResolutionHeight, result.camera.ResolutionWidth, ImageFormat.Png, writeDebugInfo, result.frameTime, result.camera);

                // record last frametime for time approximation
                lastFrameTime = frameTime;
            }

            Console.Clear();
            Console.WriteLine("Done.");
            Console.ReadKey();
        }

        static string GetProgressBar(int segments, double currentPercentage)
        {
            int completedSegments = (int)(currentPercentage / (100d / (double)segments));
            return '[' + new string('#', completedSegments) + new string('-', segments - completedSegments) + ']';
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
