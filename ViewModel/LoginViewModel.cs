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
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
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

                // Check for corresponding .myspray file
                string sprayFilePath = System.IO.Path.ChangeExtension(openFileDialog.FileName, ".myspray");
                if (File.Exists(sprayFilePath))
                {
                    LoadSprayData(sprayFilePath);
                }
            } else
            {
                ImageBorderVisibility = Visibility.Collapsed;
            }
            (OpenEditorCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        public void LoadSprayData(string filePath)
        {
            var sprayDataJson = File.ReadAllText(filePath);
            var sprayDataDto = JsonConvert.DeserializeObject<ObservableCollection<SprayPaintDetail>>(sprayDataJson);

            // Convert DTOs back to Shape objects
            PaintDots.Clear();
            foreach (var dotDetail in sprayDataDto)
            {
                var dot = ConvertDtoToShape(dotDetail);
                PaintDots.Add(dot);
            }
        }

        private Shape ConvertDtoToShape(SprayPaintDetail detail)
        {
            var shape = new Rectangle
            {
                Width = detail.Width,
                Height = detail.Height,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(detail.Color))
            };

            Canvas.SetLeft(shape, detail.X);
            Canvas.SetTop(shape, detail.Y);

            return shape;
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
            }
            else
            {
                MessageBox.Show("Please load image before continuing.", "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
