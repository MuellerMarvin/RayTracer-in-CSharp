using System;
using System.IO;
using DataClasses;

namespace RayTracer_in_CSharp
{
    class Program
    {
        static void Main()
        {
            #region Setup
            // Create Camera to render
            Camera camera = new Camera(400, 225); ///400x225 resolution

            // Create the PPM file and open it
            StreamWriter w = new StreamWriter(@".\uwu.ppm");

            // PPM header
            w.Write("P3\n" + camera.ResolutionWidth + " " + camera.ResolutionHeight + "\n255\n");

            // camera setup
            camera.ViewportHeight = 2.0;
            camera.FocalLength = 1.0;
            camera.Origin = new Point3(0, 0, 0);
            #endregion

            #region Render
            // Render
            for (int j = camera.ResolutionHeight - 1; j >= 0 ; --j)
            {
                // Announce progress
                Console.Write("Scanlines remaining: {0}", j);
                Console.CursorTop = 0;
                Console.CursorLeft = 0;

                for (int i = 0; i < camera.ResolutionWidth; ++i)
                {
                    double u = (double)i / (camera.ResolutionWidth - 1);
                    double v = (double)j / (camera.ResolutionHeight - 1);
                    Ray r = new Ray(camera.Origin, camera.LowerLeftCorner + u * camera.ViewportHorizontal + v * camera.ViewportVertical - camera.Origin);
                    Color3 pixelColor = RayColor(r);
                    pixelColor = pixelColor * 255;
                    w.WriteLine(pixelColor.R + " " + pixelColor.G + " " + pixelColor.B);
                }
            }


            #endregion

            #region Cleanup
            // Flush the stream to the drive and close the stream
            w.Flush();
            w.Close();
            #endregion
        }

        static Color3 RayColor(Ray ray)
        {
            double t = 0.5 * (ray.Direction.UnitVector.Y + 1.0);
            return (1.0 - t) * new Color3(1, 1, 1) + t * new Color3(0.5, 0.7, 1.0);

        }

#pragma warning disable IDE0051 // Remove unused private members
        static void WriteColor(Color3 color)
#pragma warning restore IDE0051 // Remove unused private members
        {
            Console.WriteLine(ColorToPpmLine(color));
        }

        static string ColorToPpmLine(Color3 color)
        {
            return (color.R * 255.999) + " " + (color.G * 255.999) + " " + (color.B * 255.999);
        }
    }

    class Camera
    {
        #region Properties
        #region Resolution
        /// <summary>
        /// The width of the image in pixels
        /// </summary>
        public int ResolutionWidth { get; set; }
        /// <summary>
        /// The height of the image in pixels
        /// </summary>
        public int ResolutionHeight { get; set; }
        /// <summary>
        /// resolutionX / resolutionY
        /// </summary>
        public double AspectRatio {
            get
            {
                return this.ResolutionWidth / this.ResolutionHeight;
            }
        }
        #endregion
        #region Viewport and camera

        /// <summary>
        /// Height of the viewport (default: 2)
        /// </summary>
        public double ViewportHeight
        {
            get
            {
                return _ViewportHeight;
            }
            set
            {
                _ViewportHeight = value;
                ViewportWidth = this.AspectRatio * _ViewportHeight;
            }
        }
        public double ViewportWidth { get; private set; }
        /// <summary>
        /// Gets the horizontal vector of the viewport
        /// </summary>
        public Vector3 ViewportHorizontal
        {
            get
            {
                return new Vector3(ViewportWidth, 0, 0);
            }
        }
        /// <summary>
        /// /// Gets the vertical vector of the viewport
        /// </summary>
        public Vector3 ViewportVertical
        {
            get
            {
                return new Vector3(0, ViewportHeight, 0);
            }
        }
        /// <summary>
        /// Gets the lower left corner of the viewport as a vector
        /// </summary>
        public Vector3 LowerLeftCorner
        {
            get
            {
                return this.Origin - (ViewportHorizontal / 2) - (ViewportVertical / 2) - (new Vector3(0, 0, this.FocalLength));
            }
        }

        /// <summary>
        /// Sets the focal length (distance of viewport to camera-origin) (default: 1.0)
        /// </summary>
        public double FocalLength { get; set; } = 1.0;

        /// <summary>
        /// Sets the origin of the Camera (default: X = 0, Y = 0, Z = 0)
        /// </summary>
        public Point3 Origin { get; set; } = new Point3(0, 0, 0);

        #endregion
        #region Holder Variables
        private double _ViewportHeight = 2;
        #endregion
        #endregion

        #region Function
        public Camera(int resolutionWidth, int resolutionHeight)
        {
            this.ResolutionWidth = resolutionWidth;
            this.ResolutionHeight = resolutionHeight;
        }
        #endregion
    }
}
