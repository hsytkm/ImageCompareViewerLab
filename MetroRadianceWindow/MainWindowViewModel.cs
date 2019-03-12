using Prism.Mvvm;

namespace MetroRadianceWindow
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string FileVersion { get; } = "Ver 1.0.0.0";

        public MainWindowViewModel()
        {

        }
    }
}
