using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WatchStoreClient.Views.Guest.Pages
{
    public partial class GuestRequestView : UserControl
    {
        public GuestRequestView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
