using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

using Raytracing.DataStructures;
using Raytracing.Hittables;

namespace Raytracing
{
    public class Renderer
    {
        #region Variables
        #region Properties
        public HittableList HittableObjects { get; set; }
        #endregion


        #endregion

        #region Constructors
        public Renderer()
        {
            this.HittableObjects = new HittableList();
        }
        #endregion

        #region Functions

        #region Write Functions
        public static void WriteFrame(string filePath, Color4[] pixels, int Y, int X, ImageFormat format, bool writeDebugInfo, long frameTime, Camera camera)
        {
            Bitmap bitmap = ColorArrayToBitmap(X, Y, pixels);

            if (writeDebugInfo)
            {
                string debugString = "Frametime: " + frameTime + " ms";
                debugString += " | Res: " + X + "x" + Y + " | " + camera.SamplesPerPixel + " Samples/Pixel | " + camera.MaxBounces + " Max Bounces";

                Graphics g = Graphics.FromImage(bitmap);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(debugString, new Font("Tahoma", 8), Brushes.White, new PointF(5, Y - 15));
            }

            bitmap.Save(filePath, format);
        }

        public static Bitmap ColorArrayToBitmap(int xRes, int yRes, Color4[] pixels)
        {
            Bitmap bitmap = new(xRes, yRes, PixelFormat.Format32bppArgb);
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

        #endregion
    }
}
 