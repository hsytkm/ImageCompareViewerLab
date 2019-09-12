using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeIcons.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public bool IsFaceOn
        {
            get => _isFaceOn;
            set => SetProperty(ref _isFaceOn, value);
        }
        private bool _isFaceOn;

        public bool IsColorZone
        {
            get => _isColorZone;
            set => SetProperty(ref _isColorZone, value);
        }
        private bool _isColorZone;

        public bool IsSelectionFrame
        {
            get => _isSelectionFrame;
            set => SetProperty(ref _isSelectionFrame, value);
        }
        private bool _isSelectionFrame;

        public bool IsDiscernCase
        {
            get => _isDiscernCase;
            set => SetProperty(ref _isDiscernCase, value);
        }
        private bool _isDiscernCase;

        public MainWindowViewModel()
        {

        }

    }
}
