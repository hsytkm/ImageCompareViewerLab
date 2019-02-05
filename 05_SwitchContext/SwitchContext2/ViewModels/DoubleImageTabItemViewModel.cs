using Prism.Regions;
using SwitchContext.Common;

namespace SwitchContext.ViewModels
{
    class DoubleImageTabItemViewModel : MultiImageViewModelBase
    {
        public override int ContentCount { get; } = 2;
        public override string ImageContentRegion { get; } = "Image2ContentRegion";
        public override string Title { get; } = "Double";
        
        public DoubleImageTabItemViewModel(IRegionManager regionManager, IApplicationCommands applicationCommands)
            : base(regionManager, applicationCommands)
        {
        }

    }
}
