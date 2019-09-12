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

        public MainWindowViewModel()
        {

        }

    }
}
