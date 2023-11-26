using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Spray_Paint_Application.ViewModel;

namespace Spray_Paint_Application.View
{
    public partial class PaintSpray : Window
    {
        public PaintSpray()
        {
            InitializeComponent();

        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                var vm = DataContext as SprayViewModel;
                vm?.CanvasMouseDownCommand.Execute(e.GetPosition((Canvas)sender));
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var vm = DataContext as SprayViewModel;
                vm?.CanvasMouseMoveCommand.Execute(e.GetPosition((Canvas)sender));
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var vm = DataContext as SprayViewModel;
                vm?.CanvasMouseUpCommand.Execute(e.GetPosition((Canvas)sender));
            }
        }
    }
}
