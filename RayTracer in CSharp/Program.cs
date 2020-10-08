﻿using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using DataClasses;
using System.Drawing;
using System.Net.WebSockets;

namespace RayTracer_in_CSharp
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Running...");
            Stopwatch sw = new Stopwatch();

            #region Setup
            string filePath = @".\rendered\image.ppm";

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
            for (int j = camera.ResolutionHeight - 1; j >= 0; --j)
            {
                for (int i = 0; i < camera.ResolutionWidth; ++i)
                {
                    double u = (double)i / (camera.ResolutionWidth - 1);
                    double v = (double)j / (camera.ResolutionHeight - 1);
                    Ray ray = new Ray(camera.Origin, camera.LowerLeftCorner + u * camera.ViewportHorizontal + v * camera.ViewportVertical - camera.Origin);
                    Color3 pixelColor = BackgroundGradient(ray);
                    pixelColor = pixelColor * 255;

                    // Render world
                    HitRecord record;
                    bool hit = world.Hit(ray, double.MinValue, double.MaxValue, out record);
                    if (hit)
                    {
                        Vector3 normal = record.Normal;
                        pixelColor = (Color3)((normal + new Vector3(1, 1, 1)) * 0.5); // scale it from 0 to 1
                        pixelColor = pixelColor * 255;
                    }

                    pixels[j * camera.ResolutionWidth + i] = pixelColor;
                }
            }
            sw.Stop();
            #endregion

            #region Save to disk
            // Create the PPM file and open it
            StreamWriter w = new StreamWriter(filePath);

            // PPM header
            w.Write("P3\n" + camera.ResolutionWidth + " " + camera.ResolutionHeight + "\n255\n");

            // Write pixels to stream
            foreach (Color3 pixel in pixels)
            {
                w.WriteLine(pixel.R + " " + pixel.G + " " + pixel.B);
            }

            // Flush to drive and close the stream
            w.Flush();
            w.Close();
            Console.WriteLine("Done. (Rendertime: {0} ms)", sw.ElapsedMilliseconds);
            #endregion
        }
        static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        static Color3 BackgroundGradient(Ray ray)
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

    struct HitRecord
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
    interface IHittable
    {
        public bool Hit(Ray ray, double minDist, double maxDist, out HitRecord hitRecord);
    }

    /// <summary>
    /// An object that implements a hittable function, as well as an origin
    /// </summary>
    interface IRenderObject : IHittable
    {
        public Point3 Origin { get; set; }
    }

    class HittableList : List<IHittable>, IHittable
    {
        public bool Hit(Ray ray, double minDist, double maxDist, out HitRecord hitRecord)
        {
            double closest = double.MaxValue;
            bool hit = false;
            hitRecord = new HitRecord();

            for (int i = 0; i < this.Count; i++)
            {
                // create record and hit target
                HitRecord currentRecord;
                if(this[i].Hit(ray, minDist, maxDist, out currentRecord))
                { // if hit
                    hit = true;

                    if(currentRecord.Distance < closest) // if it's closer than a previous hit
                    {
                        hitRecord = currentRecord; // then save it's record
                    }
                }
                // else
            }
            return hit;
        }
    }

    class Sphere : IRenderObject
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

            if(discriminant > 0)
            {
                double discRoot = Math.Sqrt(discriminant);


                double temp1 = (-half_b - discRoot) / a;
                double temp2 = (-half_b + discRoot) / a;
                if (temp1 < maxDist && temp1 > minDist || temp2 < maxDist && temp2 > minDist)
                {
                    hitRecord.Distance = (temp1 < maxDist && temp1 > minDist) ? temp1 : temp2;
                    hitRecord.Point = ray.PointAtDistance(hitRecord.Distance);

                    // Calculate Surface Normal
                    Vector3 outwardNormal = ((Vector3)hitRecord.Point - (Vector3)this.Origin) / this.Radius;
                    bool frontFace = (Vector3.Dot(ray.Direction, outwardNormal) < 0); // check if it's the front face
                    hitRecord.Normal = hitRecord.HitFront ? -outwardNormal : outwardNormal;
                    return true;
                }
            }
            return false;
        }
        #endregion
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
        /// ResolutionWidth / Resolution Height
        /// </summary>
        public double AspectRatio {
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
        public double ViewportWidth {
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
                return new Vector3(0, this.ViewportHeight, 0);
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

        #region Functions
        public Camera(int resolutionWidth, int resolutionHeight)
        {
            this.ResolutionWidth = resolutionWidth;
            this.ResolutionHeight = resolutionHeight;
            this.ViewportHeight = 2;
        }
        #endregion
    }
}

