using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Spray_Paint_Application.Model;
using Spray_Paint_Application.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Spray_Paint_Application.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private ImageModel _imageData = new ImageModel();
        public ImageModel ImageData
        {
            get => _imageData;
            set
            {
                _imageData = value;
                OnPropertyChanged(nameof(ImageData));
            }
        }

        private Visibility _imageBorderVisibility = Visibility.Collapsed;
        public Visibility ImageBorderVisibility
        {
            get => _imageBorderVisibility;
            set
            {
                if (_imageBorderVisibility != value)
                {
                    _imageBorderVisibility = value;
                    OnPropertyChanged(nameof(ImageBorderVisibility));
                }
            }
        }

        private ScaleTransform _imageScaleTransform;
        public ScaleTransform ImageScaleTransform
        {
            get => _imageScaleTransform;
            set
            {
                _imageScaleTransform = value;
                OnPropertyChanged(nameof(ImageScaleTransform));
            }
        }

        private Thickness _imageMargin;
        public Thickness ImageMargin
        {
            get => _imageMargin;
            set
            {
                _imageMargin = value;
                OnPropertyChanged(nameof(ImageMargin));
            }
        }

        public ICommand LoadImageCommand { get; }
        public ICommand OpenEditorCommand { get; }

        public LoginViewModel()
        {
            LoadImageCommand = new RelayCommand(LoadImage);
            OpenEditorCommand = new RelayCommand(OpenEditor, () => CanOpenEditor());
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select an image";
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                ImageData.Photo = bitmap;
                ImageBorderVisibility = Visibility.Visible;
                UpdateImagePresentation(bitmap);
            } else
            {
                ImageBorderVisibility = Visibility.Collapsed;
            }
            (OpenEditorCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
        
        private Boolean CanOpenEditor()
        {
            return ImageData.Photo != null;
        }

        private void OpenEditor()
        {
            if (ImageData.Photo != null)
            {
                var loginWindow = Application.Current.Windows.OfType<LoginView>().FirstOrDefault();
                EditorWindow editorWindow = new EditorWindow(this.ImageData);
                loginWindow?.Close();
                editorWindow.Show();
            } else
            {
                MessageBox.Show("Please load image before continuing.", "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void UpdateImagePresentation(BitmapImage bitmap)
        {
            double canvasWidth = 800;
            double canvasHeight = 600;

            double scaleX = canvasWidth / bitmap.PixelWidth;
            double scaleY = canvasHeight / bitmap.PixelHeight;
            double scale = Math.Min(scaleX, scaleY);

            ImageScaleTransform = new ScaleTransform(scale, scale);

            double offsetX = (canvasWidth - bitmap.PixelWidth * scale) / 2;
            double offsetY = (canvasHeight - bitmap.PixelHeight * scale) / 2;
            ImageMargin = new Thickness(offsetX, offsetY, 0, 0);
        }
    }
}
