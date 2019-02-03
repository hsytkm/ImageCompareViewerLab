using Prism.Commands;

namespace SwitchContext.Common
{
    public interface IApplicationCommands
    {
        CompositeCommand SwapInnerTrackCommand { get; }
        CompositeCommand SwapOuterTrackCommand { get; }
    }

    public class ApplicationCommands : IApplicationCommands
    {
        public CompositeCommand SwapInnerTrackCommand { get; } = new CompositeCommand(true);
        public CompositeCommand SwapOuterTrackCommand { get; } = new CompositeCommand(true);
    }
}
