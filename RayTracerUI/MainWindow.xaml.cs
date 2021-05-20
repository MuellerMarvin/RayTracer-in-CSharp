// Standard Namespaces
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


// Libraries
using Raytracing;

namespace RayTracerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bitmap DisplayedBitmap
        {
            get
            {
                return _DisplayedBitmap;
            }
            set
            {
                _DisplayedBitmap = value;

                var handle = value.GetHbitmap();
                ImageSource source;
                try
                {
                    source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                catch
                {
                    return;
                }

                BackgroundToAverageDisplayed();
            }
        }
        private Color4[] DisplayedArray
        {
            get
            {
                return _DisplayedArray;
            }
        }
        private Color4[] _DisplayedArray;

        private Bitmap _DisplayedBitmap;

        Raytracer Renderer;
        Camera Camera;

        public MainWindow()
        {
            InitializeComponent();
            this.Renderer = new Raytracer();

            this.Camera = new Camera(400, 225)
            {
                ViewportHeight = 2.0,
                FocalLength = 1.0,
                SamplesPerPixel = 10,
                MaxBounces = 12,
                MultithreadedRendering = true
            };

            this.Renderer.HittableObjects = new HittableList
            {
                new Sphere(0, 1, 0, 0.5),
                new Sphere(0.5, 1, 0, 0.5),
                new Sphere(0, 1, -100.5, 100)
            };
        }

        #region Utility
        private void SetNewImageFromFramebuffer(Color4[] frameBuffer, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            this._DisplayedArray = frameBuffer;
            frameBuffer = ScaleColorArrayToColorspace(frameBuffer);

            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    Color4 color4 = frameBuffer[y * width + x];
                    bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb((int)color4.A, (int)color4.R, (int)color4.G, (int)color4.B));
                }
            }

            this.DisplayedBitmap = bitmap;
        }

        private void BackgroundToAverageDisplayed()
        {
            Color4 avrg = GetAverageColor(this.DisplayedArray);
            this.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, (byte)avrg.R, (byte)avrg.G, (byte)avrg.B));
        }

        private Color4[] ScaleColorArrayToColorspace(Color4[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i] * 255;
            }
            return array;
        }

        private Color4 GetAverageColor(Color4[] array)
        {
            Color4 color = new Color4(0, 0, 0, 0);
            for (int i = 0; i < array.Length; i++)
            {
                color += array[i];
            }
            return color / array.Length;
        }
        #endregion

        #region ButtonEvents
        private void Options_Click(object sender, RoutedEventArgs e)
        {
            CameraOptions window = new CameraOptions(this.Camera);
            window.ShowDialog();
        }
        #endregion
    }
}
