using System.Collections.Generic;
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
            this.WhenActivated(d => d(ViewModels.MainWindowViewModel.ShowSaveFileDialog.RegisterHandler(ShowSaveFileDialog)));
            this.WhenActivated(d => d(ViewModels.MainWindowViewModel.ShowOpenFolderDialog.RegisterHandler(ShowOpenFolderDialog)));
        }
        
        // Interaction Handler
        
        // TODO: Handle errors
        private async Task ShowOpenFileDialog(InteractionContext<Unit, string?> interaction)
        {
            var dialog = new OpenFileDialog();
            dialog.Filters = new List<FileDialogFilter>
            {
                new()
                {
                    Extensions =
                    {
                        "png", 
                        "jpg"
                    }
                }
            };
            dialog.AllowMultiple = false;
            var fileNames = await dialog.ShowAsync(this);
            if (fileNames != null) interaction.SetOutput(fileNames.FirstOrDefault() ?? null);
        }
        private async Task ShowSaveFileDialog(InteractionContext<Unit, string?> interaction)
        {
            var dialog = new SaveFileDialog();
            dialog.Filters = new List<FileDialogFilter>
            {
                new()
                {
                    Extensions =
                    {
                        "png", 
                        "jpg"
                    }
                }
            };
            var fileName = await dialog.ShowAsync(this);
            interaction.SetOutput(fileName ?? null);
        }
        private async Task ShowOpenFolderDialog(InteractionContext<Unit, string?> interaction)
        {
            var dialog = new OpenFolderDialog();
            var folderPath = await dialog.ShowAsync(this);
            interaction.SetOutput(folderPath ?? null);
        }
    }
}