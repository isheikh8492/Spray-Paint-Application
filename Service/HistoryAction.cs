using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Spray_Paint_Application.Service
{
    public class HistoryAction
    {
        public List<Shape> Shapes { get; set; }
        public bool IsAddition { get; set; } // True for add, false for remove
    }
}
