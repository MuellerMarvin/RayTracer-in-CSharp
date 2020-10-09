using System;
using System.IO;
using System.Diagnostics;
using DataClasses;
using RenderObjects;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Net.Http.Headers;
using System.Dynamic;
using System.Text;

namespace RayTracer_in_CSharp
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Running...");
            Stopwatch sw = new Stopwatch();

            #region Setup
            string filePath = @".\rendered\image";

            // Create Camera to render
            Camera camera = new Camera(400, 225); ///400x225 resolution

            // camera setup
            camera.ViewportHeight = 2.0;
            camera.FocalLength = 1.0;
            camera.Origin = new Point3(0, 0, 0);

            // create pixelbuffer
            Color3[] pixels = new Color3[camera.ResolutionHeight * camera.ResolutionWidth];

            // create sphere
            HittableList world = new HittableList();
            world.Add(new Sphere(0, 0, -1, 0.5));
            world.Add(new Sphere(0, -100.5, -1, 100));

            #endregion

            #region Render
            // Render
            sw.Start();
            for (int y = 0; y < camera.ResolutionHeight; y++)
            {
                for (int x = 0; x < camera.ResolutionWidth; x++)
                {
                    double u = (double)x / (camera.ResolutionWidth - 1);
                    double v = (double)y / (camera.ResolutionHeight - 1);

                    // Render world
                    HitRecord record;
                    Ray ray = new Ray(camera.Origin, camera.LowerLeftCorner + u * camera.ViewportHorizontal + v * camera.ViewportVertical);
                    Color3 pixelColor = RayColor(ray, world);


                    // write to buffer
                    pixelColor = pixelColor * 255;
                    pixels[y * camera.ResolutionWidth + x] = pixelColor;
                }
            }
            sw.Stop();
            #endregion

            #region Save to disk
            // Save image to disk
            filePath += ".png";
            ImageFormat format = ImageFormat.Png;
            WriteImageJpg(filePath, pixels, camera.ResolutionHeight, camera.ResolutionWidth, format, true, sw.ElapsedMilliseconds);

            // open image in the user's image editor
            #region because apparently System.Diagnostics.Process.Start() doesn't want to work anymore
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(filePath)
            {
                UseShellExecute = true
            };
            p.Start();
            #endregion

            Console.WriteLine("Done. (Rendertime: {0} ms)", sw.ElapsedMilliseconds);
            //Console.ReadKey();
            #endregion
        }

        static Color3 RayColor(Ray ray, IHittable hittable)
        {
            HitRecord hitRecord;
            if (hittable.Hit(ray, 0, double.PositiveInfinity, out hitRecord))
            {
                return 0.5 * ((Color3)hitRecord.Normal + new Color3(1, 1, 1));
            }
            double t = 0.5 * (ray.Direction.UnitVector.Y + 1);
            return (1 - t) * new Color3(1, 1, 1) + t * new Color3(0.5, 0.7, 1.0);
        }

        static void WriteImageJpg(string filePath, Color3[] pixels, int height, int width, ImageFormat format, bool writeDebugInfo, long frameTime)
        {
            Bitmap bitmap;
            if(writeDebugInfo)
            {
                bitmap = new Bitmap(width, height + 20, PixelFormat.Format24bppRgb);
            }
            else
            {
                bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color3 color3 = pixels[y * width + x];
                    bitmap.SetPixel(x, y, Color.FromArgb((int)color3.R, (int)color3.G, (int)color3.B));
                }
            }

            if(writeDebugInfo)
            {
                string debugString = string.Empty;
                debugString = "Frametime: " + frameTime + " ms";
                debugString += " | Resolution: " + width + "x" + height + " (" + width * height + " pixels)";

                Graphics g = Graphics.FromImage(bitmap);
                g.DrawString(debugString, new Font("Tahoma", 8), Brushes.White, new PointF(5, height + 3));
            }

            bitmap.Save(filePath, format);
        }

        static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}

