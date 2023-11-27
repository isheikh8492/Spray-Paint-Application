using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Newtonsoft.Json;
using Spray_Paint_Application.Model;
using Spray_Paint_Application.Service;
using Spray_Paint_Application.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Spray_Paint_Application.ViewModel
{
    public class SelectViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler SprayDataLoaded;
        public ObservableCollection<Shape> PaintDots { get; } = new ObservableCollection<Shape>();

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
        private double _imageWidth;
    public double ImageWidth
    {
        get => _imageWidth;
        set
        {
            _imageWidth = value;
            OnPropertyChanged(nameof(ImageWidth));
        }
    }

    private double _imageHeight;
    public double ImageHeight
    {
        get => _imageHeight;
        set
        {
            _imageHeight = value;
            OnPropertyChanged(nameof(ImageHeight));
        }
    }

        public ICommand LoadImageCommand { get; }
        public ICommand OpenEditorCommand { get; }

        public SelectViewModel()
        {
            LoadImageCommand = new RelayCommand(LoadImage);
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

                // Open EditorWindow with the selected image
                OpenEditorWithImage(openFileDialog.FileName);
            }
            else
            {
                ImageBorderVisibility = Visibility.Collapsed;
            }
        }

        private void OpenEditorWithImage(string imagePath)
        {
            var editorViewModel = new EditorViewModel();
            editorViewModel.Initialize(new ImageModel { Photo = ImageData.Photo });

            // Check for corresponding .myspray file
            string sprayFilePath = System.IO.Path.ChangeExtension(imagePath, ".myspray");
            if (File.Exists(sprayFilePath))
            {
                editorViewModel.SprayViewModel.LoadSprayData(sprayFilePath);
            }

            EditorWindow editorWindow = new EditorWindow(editorViewModel);
            editorWindow.Show();

            // Optionally, close the current SelectView window if required
            var loginWindow = Application.Current.Windows.OfType<SelectView>().FirstOrDefault();
            loginWindow?.Close();
        }
    }
}
