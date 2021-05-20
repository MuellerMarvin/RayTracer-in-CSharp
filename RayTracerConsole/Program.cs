using System;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Raytracing;


namespace RayTracerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Camera camera = new Camera(1920, 1080)
            {
                Origin = new Vector3(0, 0, 0),
                MultithreadedRendering = true,
                SamplesPerPixel = 100,
                MaxBounces = 12
            };
            Raytracer renderer = new Raytracer();

            renderer.HittableObjects = new HittableList
            {
                new Sphere(0, 1, 0, 0.5),
                new Sphere(0.5, 1, 0, 0.5),
                new Sphere(0, 1, -100.5, 100)
            };

            const int frames = 10;
            Task<RenderResult>[] tasks = new Task<RenderResult>[frames];
            for (int i = 0; i < frames; i++)
            {
                Camera localCam = new Camera(camera.ResolutionWidth, camera.ResolutionHeight)
                {
                    Origin = camera.Origin + new Vector3(0.01f * i, 0, 0),
                    MultithreadedRendering = false,
                    SamplesPerPixel = camera.SamplesPerPixel,
                    MaxBounces = 12
                };
                RenderResult result = new RenderResult()
                {
                    frameNumber = i,
                    camera = localCam
                };

                tasks[i] = Task.Run(() =>
                {
                    result.pixels = renderer.RenderScene(localCam, out long frameTime);
                    result.frameTime = frameTime;
                    return result;
                });
            }
            Task.WaitAll(tasks);

            for (int i = 0; i < frames; i++)
            {
                renderer.WriteFrame("./images/image_" + tasks[i].Result.frameNumber + ".png", tasks[i].Result.pixels, tasks[i].Result.camera.ResolutionHeight, tasks[i].Result.camera.ResolutionWidth, ImageFormat.Png, true, tasks[i].Result.frameTime, tasks[i].Result.camera);
            }
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
