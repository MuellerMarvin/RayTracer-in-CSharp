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
            #region Settings
            string filePath = @".\rendered\image";
            #endregion

            // Create Camera to render
            Camera camera = new Camera(1280, 720) ///400x225 resolution
            {
                ViewportHeight = 2.0,
                FocalLength = 1.0,
                Origin = new Point3(0, 0, 0),
                TransparentBackground = false
            };

            // create sphere
            HittableList world = new HittableList
            {
                new Sphere(0, -1, 0, 0.5),
                new Sphere(0, -1, -100.5, 100)
            };

            #endregion

            for (int i = 0; i < 20; i++)
            {
                string pathChan = filePath;
                #region Render
                sw.Reset();
                // Render
                sw.Start();
                RenderScene(camera, world, out Color4[] pixels);
                sw.Stop();
                #endregion

                #region Save to disk
                // Save image to disk
                pathChan += i + ".png";
                ImageFormat format = ImageFormat.Png;
                WriteImage(pathChan, pixels, camera.ResolutionHeight, camera.ResolutionWidth, format, true, sw.ElapsedMilliseconds);
                camera.Origin.X += 0.1;
            }
            return;

            // open image in the user's image editor
            #region because apparently System.Diagnostics.Process.Start() doesn't want to work anymore
            var p = new Process
            {
                StartInfo = new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                }
            };
            p.Start();
            #endregion

            Console.WriteLine("Done. (Rendertime: {0} ms)", sw.ElapsedMilliseconds);
            //Console.ReadKey();
            #endregion
        }

        static void RenderScene(Camera camera, IHittable hittableObjects, out Color4[] pixelBuffer)
        {
            // set up buffer
            pixelBuffer = new Color4[camera.ResolutionHeight * camera.ResolutionWidth];

            for (int j = camera.ResolutionHeight - 1; j >= 0; --j)
            {
                for (int i = 0; i < camera.ResolutionWidth; ++i)
                {
                    Color4 pixelColor = new Color4(0, 0, 0, 0);
                    double u = (double)i / (camera.ResolutionWidth - 1);
                    double v = (double)j / (camera.ResolutionHeight - 1);

                    // Render world
                    Ray ray = new Ray(camera.Origin, camera.LowerLeftCorner + u * camera.ViewportHorizontal + v * camera.ViewportVertical);

                    if (hittableObjects.Hit(ray, 0, double.PositiveInfinity, out HitRecord hitRecord))
                    {
                        pixelColor = 0.5 * ((Color3)hitRecord.Normal + new Color4(1, 1, 1, 1));
                        pixelColor.A = 1;
                        double temp = pixelColor.B;
                        pixelColor.B = pixelColor.G;
                        pixelColor.G = temp;
                    }
                    else if (!camera.TransparentBackground)
                    {
                        // draw a background
                        double zPos = 0.5 * (ray.Direction.UnitVector.Z + 1);
                        pixelColor = (1 - zPos) * new Color3(1, 1, 1) + zPos * new Color4(0.5, 0.7, 1.0, 1);
                        pixelColor.A = 1;
                    }


                    // write to buffer
                    pixelColor *= 255;
                    pixelBuffer[j * camera.ResolutionWidth + i] = pixelColor;
                }
            }
        }

        static void WriteImage(string filePath, Color4[] pixels, int height, int width, ImageFormat format, bool writeDebugInfo, long frameTime)
        {
            Bitmap bitmap;
            if(writeDebugInfo)
            {
                bitmap = new Bitmap(width, height + 20, PixelFormat.Format32bppArgb);
            }
            else
            {
                bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            }

            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    Color4 color4 = pixels[y * width + x];
                    bitmap.SetPixel(x, y, Color.FromArgb((int) color4.A, (int)color4.R, (int)color4.G, (int)color4.B));
                }
            }

            if(writeDebugInfo)
            {
                string debugString = "Frametime: " + frameTime + " ms";
                debugString += " | Resolution: " + width + "x" + height + " (" + width * height + " pixels)";

                Graphics g = Graphics.FromImage(bitmap);
                g.DrawString(debugString, new Font("Tahoma", 8), Brushes.White, new PointF(5, height + 3));
            }

            bitmap.Save(filePath, format);
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}

