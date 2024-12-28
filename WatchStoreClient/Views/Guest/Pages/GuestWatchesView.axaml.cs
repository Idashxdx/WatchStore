using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WatchStoreClient.Views.Guest.Pages
{
    public partial class GuestWatchesView : UserControl
    {
        public GuestWatchesView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
