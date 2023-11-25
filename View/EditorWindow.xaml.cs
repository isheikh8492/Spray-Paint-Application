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

            // Set DataContext for LoginViewModel
            var loginViewModel = (LoginViewModel)this.Resources["LoginViewModel"];
            loginViewModel.ImageData = imageData;

            // If needed, you can also access SprayViewModel here
            // var sprayViewModel = (SprayViewModel)this.Resources["SprayViewModel"];
        }
    }
}
