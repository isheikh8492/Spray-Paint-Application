using Microsoft.Win32;
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
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select an image";
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                // Load the image
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                PreviewImage.Source = bitmap;
            }
            BorderVisibility();
        }

        private void BorderVisibility()
        {
            if (PreviewImage.Source != null)
            {
                ImageBorder.Visibility = Visibility.Visible;
            }
        }

        private void OpenEditorButton_Click(object sender, RoutedEventArgs e)
        {
            if (PreviewImage.Source== null)
            {
                MessageBox.Show("Please load image before continuing.", "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);

            } else
            {
                EditorWindow editorWindow = new EditorWindow();
                editorWindow.Show();
            }
        }
    }
}
