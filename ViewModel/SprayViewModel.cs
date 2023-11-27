using DevExpress.Data.Browsing;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Spray_Paint_Application.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Spray_Paint_Application.ViewModel
{
    public enum ActiveTool
    {
        Paint,
        Erase
    }

    public class SprayViewModel : INotifyPropertyChanged
    {
        private HistoryManager _historyManager = new HistoryManager();
        private readonly DispatcherTimer _sprayTimer;
        private Point _currentPosition;
        private bool _isPainting;
        public Action<string> SaveCanvasDelegate { get; set; }
        public ObservableCollection<Shape> PaintDots { get; } = new ObservableCollection<Shape>();
        public ICommand CanvasMouseDownCommand { get; }
        public ICommand CanvasMouseMoveCommand { get; }
        public ICommand CanvasMouseUpCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand EraserCommand { get; private set; }

        public ICommand SaveImageAndSprayDataCommand { get; set; }

        private SolidColorBrush _paintColor = Brushes.Black;
        private int _brushSize = 10; // Default value
        private int _brushDensity = 10; // Default value

        private ActiveTool _activeTool = ActiveTool.Paint;
        public ActiveTool ActiveTool
        {
            get => _activeTool;
            set
            {
                _activeTool = value;
                OnPropertyChanged(nameof(ActiveTool));
                OnPropertyChanged(nameof(IsPaintActive));
                OnPropertyChanged(nameof(IsEraserActive));
            }
        }

        public bool IsPaintActive => ActiveTool == ActiveTool.Paint;
        public bool IsEraserActive => ActiveTool == ActiveTool.Erase;

        public ICommand ActivatePaintCommand { get; }
        public ICommand ActivateEraserCommand { get; }

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
            EraserCommand = new RelayCommand<Point>(ErasePaint);
            ActivatePaintCommand = new RelayCommand(() => ActiveTool = ActiveTool.Paint);
            ActivateEraserCommand = new RelayCommand(() => ActiveTool = ActiveTool.Erase);
            SaveImageAndSprayDataCommand = new RelayCommand(SaveImageAndSprayData);
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
            CommandManager.InvalidateRequerySuggested();
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
            CommandManager.InvalidateRequerySuggested();
        }

        private void CanvasMouseDown(Point position)
        {
            _isPainting = true;
            _currentPosition = position;
            if (IsEraserActive)
            {
                ErasePaint(position);
            }
            else
            {
                _sprayTimer.Start();
            }
        }

        private void CanvasMouseMove(Point position)
        {
            if (!_isPainting) return;

            _currentPosition = position;
            if (IsEraserActive)
            {
                ErasePaint(position);
            }
            else
            {
                SprayPaint(position);
            }
        }

        private void CanvasMouseUp(Point position)
        {
            _isPainting = false;
            if (IsEraserActive)
            {
                ErasePaint(position);
            }
            else
            {
                _sprayTimer.Stop();
            }
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

        public void ErasePaint(Point position)
        {
            if (!_isPainting) return;

            const double eraserWidth = 20.0; // Width of the eraser area
            const double eraserHeight = 20.0; // Height of the eraser area

            // Calculate the top-left corner of the eraser area
            double eraserLeft = position.X - eraserWidth / 2;
            double eraserTop = position.Y - eraserHeight / 2;

            var dotsToErase = PaintDots
                .OfType<Rectangle>()
                .Where(rect =>
                    Canvas.GetLeft(rect) >= eraserLeft &&
                    Canvas.GetLeft(rect) <= eraserLeft + eraserWidth &&
                    Canvas.GetTop(rect) >= eraserTop &&
                    Canvas.GetTop(rect) <= eraserTop + eraserHeight)
                .ToList();

            var shapesToErase = dotsToErase.Cast<Shape>().ToList();

            foreach (var dot in dotsToErase)
            {
                Application.Current.Dispatcher.Invoke(() => PaintDots.Remove(dot));
            }

            // Record the eraser action for undo/redo
            if (shapesToErase.Any())
            {
                _historyManager.AddAction(new HistoryAction
                {
                    Shapes = shapesToErase,
                    IsAddition = false
                });
            }
        }

        public void SaveImageAndSprayData()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpeg)|*.jpeg|BMP Image (*.bmp)|*.bmp",
                DefaultExt = "png",
                AddExtension = true,
                FileName = "MyArtwork" // Default file name
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string baseFilePath = saveFileDialog.FileName;
                SaveCanvasDelegate?.Invoke(baseFilePath);
                SaveSprayData(baseFilePath);
            }
        }

        private void SaveSprayData(string baseFilePath)
        {
            string sprayDataPath = System.IO.Path.ChangeExtension(baseFilePath, ".myspray");

            var sprayDotsDto = PaintDots.Select(dot => new SprayPaintDetail
            {
                X = Canvas.GetLeft(dot),
                Y = Canvas.GetTop(dot),
                Width = dot.Width,
                Height = dot.Height,
                Color = (dot.Fill as SolidColorBrush)?.Color.ToString()
            }).ToList();

            var sprayDataJson = JsonConvert.SerializeObject(sprayDotsDto);
            File.WriteAllText(sprayDataPath, sprayDataJson);
        }

        public void LoadSprayData(string filePath)
        {
            var sprayDataJson = File.ReadAllText(filePath);
            var sprayDataDto = JsonConvert.DeserializeObject<ObservableCollection<SprayPaintDetail>>(sprayDataJson);

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


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
