using ImageMetaExtractorApp.Models;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;

namespace ImageMetaExtractorApp.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private const string ImageSource1 = @"C:\data\Image1.JPG";
        private const string ImageSource2 = @"C:\data\Image2.JPG";

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

        private void AddTab1() => _modelMaster.UpdateImage(ImageSource1);
        private void AddTab2() => _modelMaster.UpdateImage(ImageSource2);
        private void ClearAllMarks() => _modelMaster.ClearAllMarks();
    }
}
