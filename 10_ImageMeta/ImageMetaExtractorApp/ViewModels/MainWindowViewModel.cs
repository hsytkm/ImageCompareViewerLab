using ImageMetaExtractorApp.Models;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;

namespace ImageMetaExtractorApp.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private readonly static string[] ImageSource = new string[]
        {
            @"C:\data\Image1.JPG",
            @"C:\data\Image2.JPG",
            @"C:\data\maker\_Canon.JPG",
            @"C:\data\maker\_Nikon.JPG",
        };

        private readonly IRegionManager _regionManager;
        private readonly ModelMaster _modelMaster;

        public DelegateCommand AddTab1Command { get; }
        public DelegateCommand AddTab2Command { get; }
        public DelegateCommand ClearAllMarksCommand { get; }
        
        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _modelMaster = container.Resolve<ModelMaster>();

            AddTab1Command = new DelegateCommand(AddTab1);
            AddTab2Command = new DelegateCommand(AddTab2);
            ClearAllMarksCommand = new DelegateCommand(ClearAllMarks);
        }

        private void AddTab1() => _modelMaster.UpdateImage(ImageSource[2]);
        private void AddTab2() => _modelMaster.UpdateImage(ImageSource[0]);
        private void ClearAllMarks() => _modelMaster.ClearAllMarks();
    }
}
