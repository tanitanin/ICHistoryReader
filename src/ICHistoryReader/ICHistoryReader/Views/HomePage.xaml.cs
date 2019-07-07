using System;

using ICHistoryReader.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ICHistoryReader.Views
{
    public sealed partial class HomePage : Page
    {
        public HomeViewModel ViewModel { get; } = new HomeViewModel();

        public HomePage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.InitializeAsync();
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            await ViewModel.UninitializeAsync();
            base.OnNavigatedFrom(e);
        }

    }
}
