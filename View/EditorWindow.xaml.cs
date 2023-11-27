using Spray_Paint_Application.Model;
using Spray_Paint_Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Spray_Paint_Application.View
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        public EditorWindow(EditorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.SprayViewModel.SaveCanvasDelegate = SaveCanvasAsImage;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(paintCanvas);
            var editorViewModel = DataContext as EditorViewModel;
            var sprayViewModel = editorViewModel?.SprayViewModel;
            sprayViewModel?.CanvasMouseDownCommand.Execute(position);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(paintCanvas);
                var editorViewModel = DataContext as EditorViewModel;
                var sprayViewModel = editorViewModel?.SprayViewModel;
                sprayViewModel?.CanvasMouseMoveCommand.Execute(position);
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(paintCanvas);
            var editorViewModel = DataContext as EditorViewModel;
            var sprayViewModel = editorViewModel?.SprayViewModel;
            sprayViewModel?.CanvasMouseUpCommand.Execute(position);
        }

        private void EditableImage_Loaded(object sender, RoutedEventArgs e)
        {
            var image = sender as Image;
            if (image != null && image.Source != null)
            {
                paintCanvas.Width = image.ActualWidth;
                paintCanvas.Height = image.ActualHeight;
                paintCanvas.Clip = new RectangleGeometry(new Rect(0, 0, image.ActualWidth, image.ActualHeight));
            }
        }

        private void SaveCanvasAsImage(string imagePath)
        {
            // Assuming that ImageData.Photo holds the original image
            var bitmapImage = ((EditorViewModel)DataContext).LoginViewModel.ImageData.Photo;

            // Create a new BitmapEncoder based on the file extension
            BitmapEncoder encoder = GetEncoder(imagePath);

            // Create a frame from the BitmapImage and add to the encoder
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)bitmapImage));

            // Save the file
            using (var fileStream = System.IO.File.Create(imagePath))
            {
                encoder.Save(fileStream);
            }
        }

        private BitmapEncoder GetEncoder(string imagePath)
        {
            string extension = System.IO.Path.GetExtension(imagePath).ToLower();
            switch (extension)
            {
                case ".jpeg":
                    return new JpegBitmapEncoder();
                case ".bmp":
                    return new BmpBitmapEncoder();
                default:
                    return new PngBitmapEncoder();
            }
        }

        private void OnSprayDataLoaded(object sender, EventArgs e)
        {
            if (DataContext is SelectViewModel viewModel)
            {
                // Clear existing dots
                var existingDots = paintCanvas.Children.OfType<Shape>().ToList();
                foreach (var dot in existingDots)
                {
                    paintCanvas.Children.Remove(dot);
                }

                // Add new dots
                foreach (var dot in viewModel.PaintDots)
                {
                    paintCanvas.Children.Add(dot);
                }
            }
        }

    }
}
