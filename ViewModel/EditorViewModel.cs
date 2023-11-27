using DevExpress.Data.Browsing;
using Spray_Paint_Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spray_Paint_Application.ViewModel
{
    public class EditorViewModel
    {
        public SprayViewModel SprayViewModel { get; }
        public SelectViewModel LoginViewModel { get; }

        public EditorViewModel()
        {
            SprayViewModel = new SprayViewModel();
            LoginViewModel = new SelectViewModel();
        }

        public void Initialize(ImageModel imageData)
        {
            LoginViewModel.ImageData = imageData;
        }
    }
}
