using System;
using System.Collections.Generic;
using System.Text;

namespace Raytracing.DataStructures
{
    public class Camera
    {
        #region Properties


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
                return this.Resolution.AspectRatio * this.ViewportHeight;
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
        public Resolution Resolution { get; set; }
        public bool TransparentBackground { get; set; } = false;
        public int SamplesPerPixel { get; set; } = 64;
        public int MaxBounces { get; set; } = 12;
        public bool MultithreadedRendering { get; set; } = true;
        public int TileHeight { get; set; }
        public int TileWidth { get; set; }
        #endregion

        #endregion

        #region Functions
        public Camera(int xResolution, int yResolution)
        {
            this.Resolution = new Resolution(xResolution, yResolution);
            this.ViewportHeight = 2;
        }

        public Ray GetRay(double posX, double posY)
        {
            double u = (posX / (this.Resolution.X - 1));
            double v = (posY / (this.Resolution.Y - 1));
            return new Ray(this.Origin, this.LowerUpperCorner + u * this.ViewportHorizontal + v * this.ViewportVertical - this.Origin);
        }
        #endregion
    }
}
