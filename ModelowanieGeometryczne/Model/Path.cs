using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelowanieGeometryczne.Model
{
    class Path
    {
        public ObservableCollection<Point> PathVertices = new ObservableCollection<Point>();

        public Path()
        {   //4 dolne, ... 4 górne

        }
    }
}
