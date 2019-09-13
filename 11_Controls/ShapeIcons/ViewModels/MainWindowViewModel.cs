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

        public bool IsTekito1
        {
            get => _isTekito1;
            set => SetProperty(ref _isTekito1, value);
        }
        private bool _isTekito1;

        public bool IsTekito2
        {
            get => _isTekito2;
            set => SetProperty(ref _isTekito2, value);
        }
        private bool _isTekito2;

        public bool IsTekito3
        {
            get => _isTekito3;
            set => SetProperty(ref _isTekito3, value);
        }
        private bool _isTekito3;

        public bool IsTekito4
        {
            get => _isTekito4;
            set => SetProperty(ref _isTekito4, value);
        }
        private bool _isTekito4;

        public MainWindowViewModel()
        {

        }

    }
}
