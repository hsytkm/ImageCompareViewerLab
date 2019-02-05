using Prism.Commands;

namespace SwitchContext.Common
{
    // モジュールを別プロジェクトに分けている場合、
    // これらは更に別プロジェクトに分ける必要がある(12-UsingCompositeCommands)
    interface IApplicationCommands
    {
        CompositeCommand SwapInnerTrackCommand { get; }
        CompositeCommand SwapOuterTrackCommand { get; }
    }

    class ApplicationCommands : IApplicationCommands
    {
        public CompositeCommand SwapInnerTrackCommand { get; } = new CompositeCommand(true);
        public CompositeCommand SwapOuterTrackCommand { get; } = new CompositeCommand(true);
    }
}
