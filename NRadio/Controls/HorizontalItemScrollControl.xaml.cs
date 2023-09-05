using System.Collections.Generic;
using NRadio.Models;
using NRadio.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NRadio.Controls
{
    public sealed partial class HorizontalItemScrollControl : UserControl
    {
        public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(List<RadioStation>), typeof(HorizontalItemScrollControl), new PropertyMetadata(null));

        HorizontalItemScrollViewModel ViewModel = ((App)Application.Current).ViewModelLocator.HorizontalItemScrollVM;

        public HorizontalItemScrollControl()
        {
            InitializeComponent();
        }

        public List<RadioStation> Source
        {
            get => (List<RadioStation>)GetValue(SourceProperty);
            set
            {
                SetValue(SourceProperty, value);
                ViewModel.Source = value;
            }
        }
    }
}
