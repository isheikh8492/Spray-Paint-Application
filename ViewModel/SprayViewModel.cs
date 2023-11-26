using GalaSoft.MvvmLight.CommandWpf;
using Spray_Paint_Application.Service;
using System;
using System.Collections.Generic;
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
        private HistoryManager _historyManager = new HistoryManager();
        private readonly DispatcherTimer _sprayTimer;
        private Point _currentPosition;
        private bool _isPainting;

        public ObservableCollection<Shape> PaintDots { get; } = new ObservableCollection<Shape>();
        public ICommand CanvasMouseDownCommand { get; }
        public ICommand CanvasMouseMoveCommand { get; }
        public ICommand CanvasMouseUpCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        private SolidColorBrush _paintColor = Brushes.Black;
        private int _brushSize = 10; // Default value
        private int _brushDensity = 10; // Default value
        public SolidColorBrush PaintColor
        {
            get => _paintColor;
            set
            {
                _paintColor = value;
                OnPropertyChanged(nameof(PaintColor));
            }
        }

        public int BrushSize
        {
            get => _brushSize;
            set
            {
                if (_brushSize != value)
                {
                    _brushSize = value;
                    OnPropertyChanged(nameof(BrushSize));
                }
            }
        }

        public int BrushDensity
        {
            get => _brushDensity;
            set
            {
                if (_brushDensity != value)
                {
                    _brushDensity = value;
                    OnPropertyChanged(nameof(BrushDensity));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SprayViewModel()
        {
            _sprayTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            _sprayTimer.Tick += (s, e) => SprayPaint(_currentPosition);

            CanvasMouseDownCommand = new RelayCommand<Point>(CanvasMouseDown);
            CanvasMouseMoveCommand = new RelayCommand<Point>(CanvasMouseMove);
            CanvasMouseUpCommand = new RelayCommand<Point>(CanvasMouseUp);
            UndoCommand = new RelayCommand(PerformUndo, CanPerformUndo);
            RedoCommand = new RelayCommand(PerformRedo, CanPerformRedo);
        }

        private bool CanPerformUndo()
        {
            return _historyManager.UndoStack.Count > 0;
        }

        private bool CanPerformRedo()
        {
            return _historyManager.RedoStack.Count > 0;
        }


        private void PerformUndo()
        {
            var action = _historyManager.Undo();
            if (action != null)
            {
                foreach (var shape in action.Shapes)
                {
                    if (action.IsAddition)
                        PaintDots.Remove(shape); // Undoing addition
                    else
                        PaintDots.Add(shape); // Undoing removal
                }
            }
        }

        private void PerformRedo()
        {
            var action = _historyManager.Redo();
            if (action != null)
            {
                foreach (var shape in action.Shapes)
                {
                    if (action.IsAddition)
                        PaintDots.Add(shape); // Redoing addition
                    else
                        PaintDots.Remove(shape); // Redoing removal
                }
            }
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
            int dotsToCreate = BrushDensity; // More dots for higher density
            int radius = BrushSize; // Radius of the spray circle
            var addedDots = new List<Shape>(); // List to store created dots

            for (int i = 0; i < dotsToCreate; i++)
            {
                // Random angle and distance within the spray circle
                double angle = new Random().NextDouble() * Math.PI * 2;
                double distance = new Random().NextDouble() * radius;

                // Calculate the position based on the angle and distance
                Point dotPosition = new Point(
                    position.X + distance * Math.Cos(angle),
                    position.Y + distance * Math.Sin(angle));

                // Create a dot and add it to the canvas
                Rectangle dot = new Rectangle
                {
                    Width = 1,
                    Height = 1,
                    Fill = PaintColor
                };

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Canvas.SetLeft(dot, dotPosition.X);
                    Canvas.SetTop(dot, dotPosition.Y);
                    PaintDots.Add(dot);
                });

                addedDots.Add(dot); // Add the dot to the list of created dots
            }

            // Record the spray action as a HistoryAction
            _historyManager.AddAction(new HistoryAction
            {
                Shapes = addedDots,
                IsAddition = true
            });
        }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
