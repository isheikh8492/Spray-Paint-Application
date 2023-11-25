using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Spray_Paint_Application.ViewModel
{
    public class SprayViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _sprayTimer;
        private Point _currentPosition;
        private bool _isPainting;

        public ObservableCollection<Shape> PaintDots { get; } = new ObservableCollection<Shape>();
        public ICommand CanvasMouseDownCommand { get; }
        public ICommand CanvasMouseMoveCommand { get; }
        public ICommand CanvasMouseUpCommand { get; }

        private SolidColorBrush _paintColor = Brushes.Black;
        public SolidColorBrush PaintColor
        {
            get => _paintColor;
            set
            {
                _paintColor = value;
                OnPropertyChanged(nameof(PaintColor));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SprayViewModel()
        {
            _sprayTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(15)
            };
            _sprayTimer.Tick += (s, e) => SprayPaint(_currentPosition);

            CanvasMouseDownCommand = new RelayCommand<Point>(CanvasMouseDown);
            CanvasMouseMoveCommand = new RelayCommand<Point>(CanvasMouseMove);
            CanvasMouseUpCommand = new RelayCommand<Point>(CanvasMouseUp);
        }

        private void CanvasMouseDown(Point position)
        {
            _isPainting = true;
            _currentPosition = position;
            _sprayTimer.Start();
        }

        private void CanvasMouseMove(Point position)
        {
            _currentPosition = position;
        }

        private void CanvasMouseUp(Point position)
        {
            _isPainting = false;
            _sprayTimer.Stop();
        }

        private void SprayPaint(Point position)
        {
            if (!_isPainting) return;

            Random rand = new Random();
            for (int i = 0; i < 10; i++) // Number of dots to create for each spray action
            {
                double angle = rand.NextDouble() * Math.PI * 2;
                double radius = rand.NextDouble() * 10; // Range of spray effect

                Ellipse dot = new Ellipse
                {
                    Width = 2, // Small dot size
                    Height = 2,
                    Fill = PaintColor // Use the current paint color
                };

                double offsetX = Math.Cos(angle) * radius;
                double offsetY = Math.Sin(angle) * radius;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Canvas.SetLeft(dot, position.X + offsetX);
                    Canvas.SetTop(dot, position.Y + offsetY);
                    PaintDots.Add(dot);
                });
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
