using ModelowanieGeometryczne.Model;
using OpenTK.Graphics.OpenGL;

namespace ModelowanieGeometryczne.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _text;
        private Torus _torus;
        private Scene _scene;
        
        public MainViewModel()
        {
            Text = "test";
            Torus = new Torus();
            Scene = new Scene();
        }

        public Scene Scene
        {
            get { return _scene; }
            set
            {
                _scene = value;
                OnPropertyChanged("Scene");
            }
        }
        public string Text
        {

            get
            {
                return _text;
            }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public Torus Torus
        {
            get
            {
                return _torus;
            }
            set
            {
                _torus = value;
                OnPropertyChanged("Torus");
            }
        }



        internal void Render()
        {
            _scene.Render();

        }
        
    }
}

