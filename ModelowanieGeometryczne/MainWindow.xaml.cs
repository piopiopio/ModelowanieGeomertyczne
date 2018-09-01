using ModelowanieGeometryczne.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using OpenTK;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using System.Windows.Forms;


//Poruszanie kursorem: 8 y+; 2 y-; 4 x+; 6x-; - z-; + z+;

//Dodawanie punktu w miejscu kursora: 5

//Wł/Wył przesuwanie punktu kursorem

//Zeznacz punkty w pobliżu: Enter

//Kasuj punkty: Del


namespace ModelowanieGeometryczne
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private fields
        private GLControl _glControl;
        private MainViewModel _mainViewModel;
        private const double increment = 0.1;
        #endregion Private fields

        public MainWindow()
        {
            InitializeComponent();
            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;
            _mainViewModel.Scene.RefreshScene += Scene_RefreshScene;
            _mainViewModel.Scene.Torus.RefreshTorus += Torus_RefreshTorus;

        }

        #region Public Properties
        #endregion Public Properties

        #region Private Methods
        void Scene_RefreshScene(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Paint();
        }

        private void OpenTkControl_Initialized(object sender, EventArgs e)
        {
            _glControl = new GLControl();
            _glControl.MakeCurrent();
            _glControl.Paint += _glControl_Paint;
            _glControl.Dock = DockStyle.Fill;
            _glControl.MouseUp += _glControl_MouseUp;
            _glControl.MouseWheel += _glControl_MouseWheel;
            _glControl.MouseMove += _glControl_MouseMove;
            _glControl.MouseDown += _glControl_MouseDown;
            _glControl.KeyDown += _glControl_KeyDown;
            (sender as WindowsFormsHost).Child = _glControl;
        }




        void _glControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {

        }

        void Torus_RefreshTorus(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Paint();
        }

        void _glControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _mainViewModel.Scene.SetCurrentCoordinate(e.X, e.Y);
            _mainViewModel.Scene.SetCurrentRotation(e.X, e.Y);
            if (e.Button == MouseButtons.Left)
            {
                _mainViewModel.Scene.SelectPointByMouse();
                Paint();
            }
        }

        void _glControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _mainViewModel.Text = "x:" + e.X + " y:" + e.Y;
            //_mainViewModel.Scene.MouseCoordinates = new Tuple<int, int>(e.X, e.Y);


            if (e.Button == MouseButtons.Left)
            {

                _mainViewModel.Text = "Right";
                _mainViewModel.Scene.MouseMoveTranslate(e.X, e.Y);
                Paint();
            }
            if (e.Button == MouseButtons.Right)
            {  
                _mainViewModel.Scene.MouseMoveRotate(e.X, e.Y);
                Paint();


            }
        }

        void _glControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _mainViewModel.Scene.Scale += e.Delta / 3000.0;
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

        public void Paint()
        {
            _mainViewModel.Render();
            //_mainViewModel.Scene.YellowEllipse.Draw(0,0);
            _glControl.SwapBuffers();
        }
        #endregion Private Methods

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    _mainViewModel.Scene.DeleteSelectedPoints();
                    _mainViewModel.Scene.DeleteSelectedCurves();
                    _mainViewModel.Scene.DeleteSelectedPatches();
                    _mainViewModel.Scene.DeleteSelectedPatchesC2();
                    _mainViewModel.Scene.DeleteGregoryPatches();
                    Paint();
                    break;
                case Key.Enter:
                    _mainViewModel.Scene.SelectPointByCursor();
                    Paint();
                    break;
                case Key.NumPad0:
                    _mainViewModel.Scene.MoveSelectedPointsWithCoursor = !_mainViewModel.Scene.MoveSelectedPointsWithCoursor;
                    Paint();
                    break;
                case Key.NumPad4:
                    _mainViewModel.Scene.MoveCursor(-increment, 0, 0);
                    Paint();
                    break;
                case Key.NumPad6:
                    _mainViewModel.Scene.MoveCursor(increment, 0, 0);
                    Paint();
                    break;
                case Key.NumPad8:
                    _mainViewModel.Scene.MoveCursor(0, increment, 0);
                    Paint();
                    break;
                case Key.NumPad2:
                    _mainViewModel.Scene.MoveCursor(0, -increment, 0);
                    Paint();
                    break;
                case Key.Add:
                    _mainViewModel.Scene.MoveCursor(0, 0, increment);
                    Paint();
                    break;
                case Key.Subtract:
                    _mainViewModel.Scene.MoveCursor(0, 0, -increment);
                    Paint();
                    break;
                case Key.NumPad5:
                    _mainViewModel.Scene.AddPointByCursor();
                    Paint();
                    break;
            }
        }

        private void PointsListView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Paint();
        }

        private void PointsListView_KeyUp(object sender, KeyEventArgs e)
        {
            Paint();
        }

        private void RefreshScene(object sender, RoutedEventArgs e)
        {
            Paint();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Paint();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Paint();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Scene.LoadScene();
            _glControl.SwapBuffers();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Scene.SaveScene();

        }


        private void OpentkWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var opentkWindowRenderSize = OpentkWindow.RenderSize;

            _mainViewModel.Scene.Height = opentkWindowRenderSize.Height;
            _mainViewModel.Scene.Width = opentkWindowRenderSize.Width;
        }

        private void TrimCurve_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
