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

            // Set the DataContext for the image part of the UI to LoginViewModel
            var loginViewModel = (LoginViewModel)Resources["LoginViewModel"];
            loginViewModel.ImageData = imageData;
            DataContext = loginViewModel;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(paintCanvas);
            var sprayViewModel = (SprayViewModel)Resources["SprayViewModel"];
            sprayViewModel.CanvasMouseDownCommand.Execute(position);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(paintCanvas);
                var sprayViewModel = (SprayViewModel)Resources["SprayViewModel"];
                sprayViewModel.CanvasMouseMoveCommand.Execute(position);
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(paintCanvas);
            var sprayViewModel = (SprayViewModel)Resources["SprayViewModel"];
            sprayViewModel.CanvasMouseUpCommand.Execute(position);
        }
    }
}
