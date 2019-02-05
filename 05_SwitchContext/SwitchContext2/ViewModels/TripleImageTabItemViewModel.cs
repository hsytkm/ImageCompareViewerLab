using Prism.Regions;
using SwitchContext.Common;

namespace SwitchContext.ViewModels
{
    class TripleImageTabItemViewModel : MultiImageViewModelBase
    {
        public override int ContentCount { get; } = 3;
        public override string ImageContentRegion { get; } = "Image3ContentRegion";
        public override string Title { get; } = "Triple";
        
        public TripleImageTabItemViewModel(IRegionManager regionManager, IApplicationCommands applicationCommands)
            : base(regionManager, applicationCommands)
        {
        }

    }
}
