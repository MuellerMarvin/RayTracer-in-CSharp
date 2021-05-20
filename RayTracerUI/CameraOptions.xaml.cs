using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Raytracing;

namespace RayTracerUI
{
    /// <summary>
    /// Interaction logic for CameraOptions.xaml
    /// </summary> 
    public partial class CameraOptions : Window
    {
        public Camera Camera { get; set; }
        public CameraOptions(Camera currentCamera)
        {
            var temp = currentCamera;
            this.Camera = temp;

            // General
            XResTextBox.Text = Convert.ToString(this.Camera.ResolutionWidth);
            YResTextBox.Text = Convert.ToString(this.Camera.ResolutionHeight);

            // Rendering
            this.SamplesTextBox.Text = Convert.ToString(this.Camera.SamplesPerPixel);
            this.BouncesTextBox.Text = Convert.ToString(this.Camera.MaxBounces);

            // Performance
            this.MultithreadedCheckBox.IsChecked = this.Camera.MultithreadedRendering;

            InitializeComponent();
        }

        private void Multithreaded_Checked(object sender, RoutedEventArgs e)
        {
            this.Camera.MultithreadedRendering = true;
        }

        private void Multithreaded_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Camera.MultithreadedRendering = false;
        }

        private void SamplesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(SamplesTextBox.Text, out int result))
                this.Camera.SamplesPerPixel = result;
        }

        private void BouncesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(BouncesTextBox.Text, out int result))
                this.Camera.MaxBounces = result;
        }

        private void XResTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(XResTextBox.Text, out int result))
                this.Camera.ResolutionWidth = result;
        }

        private void YResTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(YResTextBox.Text, out int result))
                this.Camera.ResolutionHeight = result;
        }
    }
}
