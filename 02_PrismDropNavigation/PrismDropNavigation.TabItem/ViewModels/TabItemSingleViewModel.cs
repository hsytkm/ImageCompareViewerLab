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
    public class TabItemSingleViewModel : TabItemViewModelBase
    {
        public TabItemSingleViewModel() : base(1)
        {
            Message = nameof(TabItemSingleViewModel);
        }
    }
}
