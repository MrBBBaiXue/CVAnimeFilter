using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using CVAnimeFilter.ViewModels;
using ReactiveUI;

namespace CVAnimeFilter.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            // When the window is activated, registers a handler for the ShowOpenFileDialog interaction.
            this.WhenActivated(d => d(ViewModels.MainWindowViewModel.ShowOpenFileDialog.RegisterHandler(ShowOpenFileDialog)));
        }
        
        // Interaction Handler
        private async Task ShowOpenFileDialog(InteractionContext<Unit, string?> interaction)
        {
            var dialog = new OpenFileDialog();
            var fileNames = await dialog.ShowAsync(this);
            if (fileNames != null) interaction.SetOutput(fileNames.FirstOrDefault());
        }
    }
}