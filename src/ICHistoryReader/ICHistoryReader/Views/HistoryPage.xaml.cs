using System;

using ICHistoryReader.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ICHistoryReader.Views
{
    public sealed partial class HistoryPage : Page
    {
        public HistoryViewModel ViewModel { get; } = new HistoryViewModel();

        public HistoryPage()
        {
            InitializeComponent();
        }
    }
}
