using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using System.Windows.Controls;

namespace Spray_Paint_Application.ViewModel
{
    public class PaintViewModel : INotifyPropertyChanged
    {
        // Properties to bind to UI elements for color and size
        private Color _paintColor = Colors.Black; // Default color
        private int _sprayRadius = 10; // Default radius for the spray effect
        public ObservableCollection<UIElement> PaintElements { get; } = new ObservableCollection<UIElement>();

        public ICommand CanvasMouseDownCommand { get; }
        public ICommand CanvasMouseMoveCommand { get; }
        public ICommand CanvasMouseUpCommand { get; }

        private bool _isPainting;

        public PaintViewModel()
        {
            CanvasMouseDownCommand = new RelayCommand<Point>(CanvasMouseDown);
            CanvasMouseMoveCommand = new RelayCommand<Point>(CanvasMouseMove);
            CanvasMouseUpCommand = new RelayCommand<Point>(CanvasMouseUp);
        }

        private void CanvasMouseDown(Point position)
        {
            _isPainting = true;
            SprayPaint(position);
        }

        private void CanvasMouseMove(Point position)
        {
            if (_isPainting)
            {
                SprayPaint(position);
            }
        }

        private void CanvasMouseUp(Point position)
        {
            _isPainting = false;
        }

        private void SprayPaint(Point position)
        {
            // Here you can implement an algorithm to create a spray effect.
            // For simplicity, let's just draw a circle at the position.
            var dot = new Ellipse
            {
                Width = _sprayRadius,
                Height = _sprayRadius,
                Fill = new SolidColorBrush(_paintColor)
            };

            // Set the position of the dot
            Canvas.SetLeft(dot, position.X - _sprayRadius / 2);
            Canvas.SetTop(dot, position.Y - _sprayRadius / 2);

            // Add the dot to the collection
            PaintElements.Add(dot);
        }

        // Implement INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
