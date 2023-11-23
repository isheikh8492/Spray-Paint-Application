using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System;
using Spray_Paint_Application.Model;

namespace Spray_Paint_Application.ViewModel
{
    public class ColorViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ColorModel> ColorChoices { get; set; }

        private ColorModel _selectedColorModel;
        public ColorModel SelectedColorModel
        {
            get { return _selectedColorModel; }
            set
            {
                if (_selectedColorModel != value)
                {
                    _selectedColorModel = value;
                    OnPropertyChanged(nameof(SelectedColorModel));
                    UpdateColorSelection(value);
                }
            }
        }

        public ICommand SelectColorCommand { get; private set; }

        public ColorViewModel() => ColorChoices = new ObservableCollection<ColorModel>
        {
            new ColorModel(Colors.Black),
            new ColorModel(Colors.White),
            // ... other colors
            new ColorModel(Colors.AliceBlue),
            new ColorModel(Colors.Aqua),
            new ColorModel(Colors.Aquamarine),
        };

        private void UpdateColorSelection(ColorModel selectedColorModel)
        {
            foreach (var colorModel in ColorChoices)
            {
                colorModel.IsSelected = colorModel == selectedColorModel;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateColorSelection()
        {
            foreach (var color in ColorChoices)
            {   

                // Assuming you have a property in your color model that controls the border
                color.IsSelected = color.Equals(SelectedColorModel);
            }
        }
    }

    // A simple implementation of ICommand
    public class RelayCommand<T> : ICommand
    {
        private Action<T> _execute;

        public RelayCommand(Action<T> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
