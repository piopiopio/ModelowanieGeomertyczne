using ModelowanieGeometryczne.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenTK;
using MessageBox = System.Windows.Forms.MessageBox;

namespace ModelowanieGeometryczne
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GLControl _glControl;
        private MainViewModel _mainViewModel;
        private double _scale;

        public MainWindow()
        {
            InitializeComponent();
            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;
            _scale = 0.1;
        }

        public double Scale
        {
            get { return _scale; }
            set
            {
                _scale = Math.Max(value, 0.01);
            }
        }
        private void OpenTkControl_Initialized(object sender, EventArgs e)
        {
            _glControl = new GLControl();
            _glControl.MakeCurrent();
            _glControl.Paint += _glControl_Paint;
            _glControl.Dock = DockStyle.Fill;
            _glControl.MouseUp += _glControl_MouseUp;
            _glControl.MouseWheel += _glControl_MouseWheel;
            (sender as WindowsFormsHost).Child = _glControl;
        }

        void _glControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // _mainViewModel.Text="MainWindow";
            Scale += e.Delta / 3000.0;

            _mainViewModel.Text = Scale.ToString() + ":" + e.Delta;

            Paint();

        }

        void _glControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //MessageBox.Show("Up");
        }

        void _glControl_Paint(object sender, PaintEventArgs e)
        {
            Paint();
        }
        void Paint()
        {
            _mainViewModel.Render(Scale);

            _glControl.SwapBuffers();
        }
    }
}
