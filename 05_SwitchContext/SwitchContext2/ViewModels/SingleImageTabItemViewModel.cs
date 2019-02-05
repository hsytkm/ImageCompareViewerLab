using Prism.Regions;
using SwitchContext.Common;

namespace SwitchContext.ViewModels
{
    class SingleImageTabItemViewModel : MultiImageViewModelBase
    {
        public override int ContentCount { get; } = 1;
        public override string Title { get; } = "Single";

        public SingleImageTabItemViewModel(IRegionManager regionManager, IApplicationCommands applicationCommands)
            : base(regionManager, applicationCommands)
        {
        }

    }
}
