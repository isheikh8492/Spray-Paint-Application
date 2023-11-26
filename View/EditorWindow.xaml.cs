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
        public EditorWindow(ImageModel imageData)
        {
            InitializeComponent();
            var viewModel = new EditorViewModel();
            viewModel.Initialize(imageData); // Make sure imageData is not null
            DataContext = viewModel;
        }


        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(paintCanvas);
            // Retrieve SprayViewModel from the DataContext which is an instance of EditorViewModel
            var editorViewModel = DataContext as EditorViewModel;
            var sprayViewModel = editorViewModel?.SprayViewModel;
            sprayViewModel?.CanvasMouseDownCommand.Execute(position);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(paintCanvas);
                // Retrieve SprayViewModel from the DataContext which is an instance of EditorViewModel
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
    }
}
