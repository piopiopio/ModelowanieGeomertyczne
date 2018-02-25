using ModelowanieGeometryczne.Model;
using OpenTK.Graphics.OpenGL;

namespace ModelowanieGeometryczne.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _text;
        
        private Scene _scene;
        
        public MainViewModel()
        {
            Text = "test";

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




        internal void Render()
        {
            _scene.Render();

        }
        
    }
}

