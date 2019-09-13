using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeIcons.ViewModels
{
    class ChannelsCheckBoxViewModel : BindableBase
    {
        // ◆RGB切り替えで他のチェンネルをOFFっていない。
        // ◆ViewModel側を作りこんでいないが、まぁいいや。
        public bool IsSingleChannel
        {
            get => _isSingleChannel;
            set => SetProperty(ref _isSingleChannel, value);
        }
        private bool _isSingleChannel;

        public bool IsChannelVisibleR
        {
            get => _isChannelVisibleR;
            set => SetProperty(ref _isChannelVisibleR, value);
        }
        private bool _isChannelVisibleR;

        public bool IsChannelVisibleG
        {
            get => _isChannelVisibleG;
            set => SetProperty(ref _isChannelVisibleG, value);
        }
        private bool _isChannelVisibleG = true;

        public bool IsChannelVisibleB
        {
            get => _isChannelVisibleB;
            set => SetProperty(ref _isChannelVisibleB, value);
        }
        private bool _isChannelVisibleB;

    }
}
