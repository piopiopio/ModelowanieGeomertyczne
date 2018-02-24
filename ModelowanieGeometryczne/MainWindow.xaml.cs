using ModelowanieGeometryczne.ViewModel;
using System;
using System.Collections.Generic;
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
        public int scrollvalue ;
        public MainWindow()
        {
            InitializeComponent();
            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;
        }

        private void OpenTkControl_Initialized(object sender, EventArgs e)
        {
            _glControl =new GLControl();
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
            scrollvalue += e.Delta;
            _mainViewModel.Text = scrollvalue.ToString();

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
        void Paint ()
        {
            _mainViewModel.Render();
            _glControl.SwapBuffers();
        }
    }
}
