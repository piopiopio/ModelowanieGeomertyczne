using ModelowanieGeometryczne.Model;
using OpenTK.Graphics.OpenGL;

namespace ModelowanieGeometryczne.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Private fields
        private string _text;
        private Scene _scene;
        #endregion Private fields

        #region Public Properties
        public MainViewModel()
        {
           

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
        #endregion Public Properties
        #region Private Methods
        internal void Render()
        {
            _scene.Render();

        }
        #endregion Private Methods
    }
}

