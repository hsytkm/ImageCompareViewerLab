using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismDropNavigation.TabItem.ViewModels
{
    public class TabItemDoubleViewModel : TabItemViewModelBase
    {
        public TabItemDoubleViewModel() : base()
        {
            Message = nameof(TabItemDoubleViewModel);
        }
    }
}
