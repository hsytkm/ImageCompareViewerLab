using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace VirtualizationListItems.Views
{
    class FastCanvas : Canvas
    {
        //private HashSet<UIElement> _virtualChildren = new HashSet<UIElement>();

        //public void AddElement(UIElement element)
        //{
        //    _virtualChildren.Add(element);
        //}

        //public void RemoveElement(UIElement element)
        //{
        //    _virtualChildren.Remove(element);
        //}

        //public void SetViewport(Rect rect)
        //{
        //    foreach (FrameworkElement child in _virtualChildren)
        //    {
        //        var childRect = new Rect(Canvas.GetLeft(child), Canvas.GetTop(child), child.Width, child.Height);

        //        if (!rect.IntersectsWith(childRect))
        //        {
        //            if (Children.Contains(child))
        //                Children.Remove(child);
        //        }
        //        else
        //        {
        //            if (!Children.Contains(child))
        //                Children.Add(child);
        //        }
        //    }
        //}

    }
}
