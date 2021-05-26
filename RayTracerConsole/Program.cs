using System;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Raytracing;
using Raytracing.DataStructures;


namespace RayTracerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            bool writeDebugInfo = false;

            Raytracer renderer = new Raytracer
            {
                // Define objects in the scene
                HittableObjects = new HittableList
                {
                    new Sphere(0, 1, 0, 0.5),
                    new Sphere(0.5, 1, 0, 0.5),
                    new Sphere(0, 1, -100.5, 100)
                }
            };

            // Define the camera
            Camera camera = new Camera(720, 480)
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
                Console.Write("{0} out of {1} completed. ({2} %) {3}", i, frames, Math.Round((float)(i) / ((float)frames / 100)), timeRemaining);

                // define camera + changes to it
                Camera localCam = new Camera(camera.ResolutionWidth, camera.ResolutionHeight)
                {
                    Origin = camera.Origin + new Vector3(0.01f * i, 0, 0),
                    MultithreadedRendering = camera.MultithreadedRendering,
                    SamplesPerPixel = camera.SamplesPerPixel,
                    MaxBounces = camera.MaxBounces
                };

                RenderResult result = new RenderResult()
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
    }

    struct RenderResult
    {
        public int frameNumber;
        public Camera camera;
        public Color4[] pixels;
        public long frameTime;
    }
}
