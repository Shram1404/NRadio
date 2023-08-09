using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace NRadio.Behaviors
{
    public class ListViewOffsetBehavior : Behavior<ListView>
    {
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ListViewOffsetBehavior),
                new PropertyMetadata(0.0, OnHorizontalOffsetChanged));

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        private static void OnHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as ListViewOffsetBehavior;
            if (behavior != null && behavior.AssociatedObject != null)
            {
                var scrollViewer = FindScrollViewer(behavior.AssociatedObject);
                if (scrollViewer != null)
                {
                    scrollViewer.ChangeView((double)e.NewValue, null, null);
                }
            }
        }

        private static ScrollViewer FindScrollViewer(DependencyObject parent)
        {
            if (parent is ScrollViewer)
            {
                return parent as ScrollViewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var result = FindScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
