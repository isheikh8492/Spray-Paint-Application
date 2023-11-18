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
                EditorWindow editorWindow = new EditorWindow(ImageData.Photo);
                loginWindow?.Close();
                editorWindow.Show();
            } else
            {
                MessageBox.Show("Please load image before continuing.", "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

    }
}
