using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

using Raytracing.DataStructures;

namespace Raytracing
{
    public class Raytracer
    {
        #region Variables
        #region Properties
        public HittableList HittableObjects { get; set; }
        #endregion

        #region Internal Variables
        private ThreadLocal<Random> RanGen = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
        #endregion
        #endregion

        #region Events
        public delegate void PixelRenderedHandler(Color4 color, int x, int y);
        #endregion

        #region Constructors
        #region pass by value
        public Raytracer()
        {
            this.HittableObjects = new HittableList();
        }

        public Raytracer(Camera camera)
        {
            camera = camera;
            this.HittableObjects = new HittableList();
        }

        public Raytracer(Camera camera, HittableList hittableObjects)
        {
            camera = camera;
            this.HittableObjects = hittableObjects;
        }
        #endregion
        #region pass by reference
        public Raytracer(ref Camera camera)
        {
            camera = camera;
            this.HittableObjects = new HittableList();
        }

        public Raytracer(ref Camera camera, ref HittableList hittableObjects)
        {
            camera = camera;
            this.HittableObjects = hittableObjects;
        }
        #endregion
        #endregion

        #region Functions
        #region RenderFunctions
        public Color4[] RenderScene(Camera camera)
        {
            return RenderScene(camera, out long frametime);
        }

        public Color4[] RenderScene(Camera camera, out long frameTime)
        {
            Stopwatch sw = new Stopwatch();
            Color4[] pixels;
            if (camera.MultithreadedRendering)
            {
                sw.Start();
                pixels = RenderSceneMultithreaded(camera);
                sw.Stop();
                frameTime = sw.ElapsedMilliseconds;
            }
            else
            {
                sw.Start();
                pixels = RenderSceneSingleThreaded(camera);
                sw.Stop();
                frameTime = sw.ElapsedMilliseconds;
            }
            return pixels;
        }

        private Color4[] RenderSceneSingleThreaded(Camera camera)
        {
            // set up buffer
            Color4[] frameBuffer = new Color4[camera.ResolutionHeight * camera.ResolutionWidth];

            // Create tasks that return the final color of a pixel
            for (int y = camera.ResolutionHeight - 1; y >= 0; --y)
            {
                for (int x = 0; x < camera.ResolutionWidth; ++x)
                {
                    frameBuffer[y * camera.ResolutionWidth + x] = RenderPixel(x, y, camera, this.HittableObjects);
                }
            }
            return frameBuffer;
        }

        private Color4[] RenderSceneMultithreaded(Camera camera)
        {
            // set up 
            Color4[] pixels = new Color4[camera.ResolutionHeight * camera.ResolutionWidth];
            Task<Color4>[] tasks = new Task<Color4>[camera.ResolutionHeight * camera.ResolutionWidth];

            // Create tasks that return the final color of a pixel
            for (int y = camera.ResolutionHeight - 1; y >= 0; --y)
            {
                for (int x = 0; x < camera.ResolutionWidth; ++x)
                {
                    var _x = x;
                    var _y = y;
                    tasks[y * camera.ResolutionWidth + x] = Task<Color4>.Run(() =>
                    {
                        return RenderPixel(_x, _y, camera, this.HittableObjects);
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
        
        public Color4[] RenderDepthMap(double maxDist, Camera camera)
        {
            Color4[] depthMap = new Color4[camera.ResolutionHeight * camera.ResolutionWidth];

            for (int y = camera.ResolutionHeight - 1; y >= 0; --y)
            {
                for (int x = 0; x < camera.ResolutionWidth; ++x)
                {
                    // get the distance
                    double distance = GetDistanceOnPixel(x, y, camera);

                    // clamp it
                    distance = Math.Clamp(distance, 0, maxDist);

                    // scale it to fit the DepthMap (0 - 1)
                    int brightness = (int)Scale(distance, 0, maxDist, 0, 1);

                    depthMap[y * camera.ResolutionWidth + x] = new Color4(brightness, brightness, brightness, 1);
                }
            }

            return depthMap;
        }

        private double GetDistanceOnPixel(double x, double y, Camera camera)
        {
            if (HittableObjects.Hit(camera.GetRay(x, y), 0, double.PositiveInfinity, out HitRecord hitRecord))
            {
                return hitRecord.Distance;
            }
            else
            {
                return double.PositiveInfinity;
            }
        }

        public Color4[] RenderNormals(Camera camera)
        {
            Color4[] normals = new Color4[camera.ResolutionHeight * camera.ResolutionWidth];

            for (int y = camera.ResolutionHeight - 1; y >= 0; --y)
            {
                for (int x = 0; x < camera.ResolutionWidth; ++x)
                {

                    Color4 pixel = new Color4(1, 1, 1, 1);

                    if(HittableObjects.Hit(camera.GetRay(x, y), 0, double.PositiveInfinity, out HitRecord hitRecord))
                    {
                        Vector3 vector = 0.5 * (hitRecord.Normal + new Vector3(1, 1, 1));
                        pixel = new Color4(vector.X, vector.Y, vector.Z, 1);
                    }

                    normals[y * camera.ResolutionWidth + x] = pixel;
                }
            }

            return normals;
        }

        private Color4 RenderPixel(double x, double y, Camera camera, HittableList hittableObject)
        {
            Color4 pixelColor = new Color4(0, 0, 0, 0);
            for (int i = 0; i < camera.SamplesPerPixel; i++)
            {
                pixelColor += GetRayColor(camera.GetRay(x + (RanGen.Value.NextDouble() * 2 - 1), y + (RanGen.Value.NextDouble() * 2 - 1)), hittableObject, camera.TransparentBackground, camera.MaxBounces);
            }

            return pixelColor / camera.SamplesPerPixel;
        }

        private Color4 GetRayColor(Ray ray, HittableList hittableObjects, bool transparentBackground, int maxBounces)
        {
            if (maxBounces < 0)
                return new Color4(0, 0, 0, 1);

            Color4 pixelColor = new Color4(0, 0, 0, 0);
            if (hittableObjects.Hit(ray, 0.001, double.PositiveInfinity, out HitRecord hitRecord))
            {
                Vector3 target = (Vector3)hitRecord.Point + hitRecord.Normal + GetRandomUnitVector();
                pixelColor = 0.5 * GetRayColor(new Ray(hitRecord.Point, target - (Vector3)hitRecord.Point), hittableObjects, false, maxBounces - 1);
                pixelColor.A = 1;
            }
            else if (!transparentBackground)
            {
                // draw a background
                double zPos = 0.5 * (ray.Direction.UnitVector.Z + 1);
                pixelColor = (1 - zPos) * new Color4(1, 1, 1, 1) + zPos * new Color4(0.5, 0.7, 1.0, 1);
                pixelColor.A = 1;
            }
            // else
            return pixelColor;
        }

        #endregion

        #region Write Functions
        public void WriteFrame(string filePath, Color4[] pixels, int height, int width, ImageFormat format, bool writeDebugInfo, long frameTime, Camera camera)
        {
            Bitmap bitmap = ColorArrayToBitmap(width, height, pixels);

            if (writeDebugInfo)
            {
                string debugString = "Frametime: " + frameTime + " ms";
                debugString += " | Res: " + width + "x" + height + " | " + camera.SamplesPerPixel + " Samples/Pixel | " + camera.MaxBounces + " Max Bounces";

                Graphics g = Graphics.FromImage(bitmap);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(debugString, new Font("Tahoma", 8), Brushes.White, new PointF(5, height - 15));
            }

            bitmap.Save(filePath, format);
        }

        public Bitmap ColorArrayToBitmap(int xRes, int yRes, Color4[] pixels)
        {
            Bitmap bitmap = new Bitmap(xRes, yRes, PixelFormat.Format32bppArgb);
            for (int y = yRes - 1; y >= 0; y--)
            {
                for (int x = 0; x < xRes; x++)
                {
                    Color4 color4 = pixels[y * xRes + x];
                    bitmap.SetPixel(x, y, Color.FromArgb((int)(color4.A * 255), (int)(color4.R * 255), (int)(color4.G * 255), (int)(color4.B * 255)));
                }
            }
            return bitmap;
        }


        #endregion

        #region Utility

        private double Scale(double value, double min, double max, double minScaled, double maxScaled)
        {
            double scaled = minScaled + (maxScaled - minScaled) * ((value - min) / (max - min));
            return scaled;
        }

        private Vector3 GetRandomUnitVector()
        {
            while (true)
            {
                Vector3 vector = new Vector3(RanGen.Value.NextDouble(), RanGen.Value.NextDouble(), RanGen.Value.NextDouble());
                if (vector.LengthSquared >= 1) continue; // continue if not in the sphere
                return vector; // else
            }
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
        #endregion
        #endregion
    }

    public struct HitRecord
    {
        /// <summary>
        /// Point of intersection
        /// </summary>
        public Point3 Point { get; set; }

        /// <summary>
        /// Surface-Normal at point of intersection
        /// </summary>
        public Vector3 Normal { get; set; }

        /// <summary>
        /// Distance from ray-origin
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// Determines if the surface was hit from its front
        /// </summary>
        public bool HitFront { get; set; }
    }

    /// <summary>
    /// An object that implements a hittable function
    /// </summary>
    public interface IHittable
    {
        public bool Hit(Ray ray, double minDist, double maxDist, out HitRecord hitRecord);
    }

    /// <summary>
    /// An object that implements a hittable function, as well as an origin
    /// </summary>
    public interface IRenderObject : IHittable
    {
        public Point3 Origin { get; set; }
    }

    public class HittableList : List<IHittable>, IHittable
    {
        public bool Hit(Ray ray, double minDist, double maxDist, out HitRecord hitRecord)
        {
            hitRecord = new HitRecord();
            double closest = double.MaxValue;
            bool hit = false;

            for (int i = 0; i < this.Count; i++)
            {
                // create record and hit target
                if (this[i].Hit(ray, minDist, maxDist, out HitRecord currentRecord))
                { // if hit
                    hit = true;
                     
                    if (currentRecord.Distance < closest) // if it's closer than a previous hit
                    {
                        hitRecord = currentRecord; // then save it's record
                        closest = currentRecord.Distance;
                    }
                }
                // else
            }
            return hit;
        }
    }

    public class Sphere : IRenderObject
    {
        #region Properties
        public Point3 Origin { get; set; } = new Point3(0, 0, 0);
        public double Radius { get; set; } = 1;
        #endregion

        #region Functions

        #region Constructors
        public Sphere()
        {
            return;
        }

        public Sphere(double radius)
        {
            this.Radius = radius;
        }

        public Sphere(double x, double y, double z)
        {
            this.Origin = new Point3(x, y, z);
        }

        public Sphere(double x, double y, double z, double radius)
        {
            this.Origin = new Point3(x, y, z);
            this.Radius = radius;
        }
        #endregion

        /// <summary>
        /// Check if a ray hits the sphere
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public bool Hit(Ray ray)
        {
            Vector3 oc = ray.Origin - (Vector3)this.Origin; // Ray Origin - Center of the Sphere = OC
            double a = ray.Direction.LengthSquared;
            double half_b = Vector3.Dot(oc, ray.Direction);
            double c = oc.LengthSquared - Radius * Radius;
            double discriminant = half_b * half_b - a * c;
            return (discriminant > 0);
        }

        public bool Hit(Ray ray, double minDist, double maxDist, out HitRecord hitRecord)
        {
            hitRecord = new HitRecord();

            Vector3 oc = ray.Origin - (Vector3)this.Origin; // Ray Origin - Center of the Sphere = OC
            double a = ray.Direction.LengthSquared;
            double half_b = Vector3.Dot(oc, ray.Direction);
            double c = oc.LengthSquared - Radius * Radius;
            double discriminant = half_b * half_b - a * c;

            if (discriminant > 0)
            {
                double root = Math.Sqrt(discriminant);
                double temp = (-half_b - root) / a;
                if (temp < maxDist && temp > minDist)
                {
                    hitRecord.Distance = temp;
                    hitRecord.Point = ray.PointAtDistance(hitRecord.Distance);
                    Vector3 outwardNormal = ((Vector3)hitRecord.Point - this.Origin) / this.Radius;

                    // determine if ray faces normal
                    hitRecord.HitFront = Vector3.Dot(ray.Direction, outwardNormal) < 0;
                    hitRecord.Normal = hitRecord.HitFront ? outwardNormal : -outwardNormal;

                    return true;
                }
                temp = (-half_b + root) / a;
                if (temp < maxDist && temp > minDist)
                {
                    hitRecord.Distance = temp;
                    hitRecord.Point = ray.PointAtDistance(hitRecord.Distance);
                    Vector3 outwardNormal = ((Vector3)hitRecord.Point - this.Origin) / this.Radius;

                    // determine if ray faces normal
                    hitRecord.HitFront = Vector3.Dot(ray.Direction, outwardNormal) < 0;
                    hitRecord.Normal = hitRecord.HitFront ? outwardNormal : -outwardNormal;
                    return true;
                }
            }
            return false;
        }
        #endregion
    }

    public class Camera
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
        /// ResolutionWidth / Resolution Height
        /// </summary>
        public double AspectRatio
        {
            get
            {
                return (double)this.ResolutionWidth / (double)this.ResolutionHeight;
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
            }
        }
        public double ViewportWidth
        {
            get
            {
                return this.AspectRatio * this.ViewportHeight;
            }
        }
        /// <summary>
        /// Gets the horizontal vector of the viewport
        /// </summary>
        public Vector3 ViewportHorizontal
        {
            get
            {
                return new Vector3(this.ViewportWidth, 0, 0);
            }
        }
        /// <summary>
        /// /// Gets the vertical vector of the viewport
        /// </summary>
        public Vector3 ViewportVertical
        {
            get
            {
                return new Vector3(0, 0, -this.ViewportHeight);
            }
        }
        /// <summary>
        /// Gets the upper left corner of the viewport as a vector
        /// </summary>
        public Vector3 LowerUpperCorner
        {
            get
            {
                return this.Origin - (ViewportHorizontal / 2) - (ViewportVertical / 2) + (new Vector3(0, this.FocalLength, 0));
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

        #region Render Config
        public bool TransparentBackground { get; set; }
        public int SamplesPerPixel { get; set; } = 10;
        public int MaxBounces { get; set; } = 50;
        public bool MultithreadedRendering { get; set; } = true;
        #endregion

        #endregion

        #region Functions
        public Camera(int resolutionWidth, int resolutionHeight)
        {
            this.ResolutionWidth = resolutionWidth;
            this.ResolutionHeight = resolutionHeight;
            this.ViewportHeight = 2;
        }

        public Ray GetRay(double posX, double posY)
        {
            double u = (posX / (this.ResolutionWidth - 1));
            double v = (posY / (this.ResolutionHeight - 1));
            return new Ray(this.Origin, this.LowerUpperCorner + u * this.ViewportHorizontal + v * this.ViewportVertical - this.Origin);
        }
    #endregion
    }
}
 