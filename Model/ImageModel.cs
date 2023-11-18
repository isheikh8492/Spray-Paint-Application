using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Spray_Paint_Application.Model
{
    public class ImageModel : INotifyPropertyChanged
    {
        private ImageSource _photo;
        public ImageSource Photo
        {
            get => _photo;
            set
            {
                if (_photo != value)
                {
                    _photo = value;
                    OnPropertyChanged(nameof(Photo));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
