using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using ReactiveUI;

namespace CVAnimeFilter.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Bitmap _mainImage;
        public Bitmap MainImage
        {
            get => _mainImage;
            set => this.RaiseAndSetIfChanged(ref _mainImage, value);
        }
        public ReactiveCommand<Unit, Unit> OpenImageCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveImageCommand { get; }
        public ReactiveCommand<Unit, Unit> BatchOperationCommand { get; }
        public static Interaction<Unit, string?> ShowOpenFileDialog { get; } = new();
        public static Interaction<Unit, string?> ShowOpenFolderDialog { get; } = new();
        public static Interaction<Unit, string?> ShowSaveFileDialog { get; } = new();
        
        // Init
        public MainWindowViewModel()
        {
            OpenImageCommand = ReactiveCommand.CreateFromTask(OpenImageAsync);
            SaveImageCommand = ReactiveCommand.CreateFromTask(SaveImageAsync);
            BatchOperationCommand = ReactiveCommand.CreateFromTask(BatchOperationAsync);
            // The ShowOpenFileDialog interaction requests the UI to show the file open dialog.
        }
        
        // Commands
        private async Task OpenImageAsync()
        {
            var fileName = await ShowOpenFileDialog.Handle(Unit.Default);

            if (fileName != null)
            {
                // load images and operating.
                var bitmap = new Bitmap(CVSharpGetEdge(fileName).ToMemoryStream());
                MainImage = bitmap;
            }
        }
        
        private async Task SaveImageAsync()
        {
            var fileName = await ShowSaveFileDialog.Handle(Unit.Default);
            
            if (fileName != null)
            {
                // save image.
                MainImage.Save(fileName);
            }
        }

        private async Task BatchOperationAsync()
        {
            var folderSource = await ShowOpenFolderDialog.Handle(Unit.Default);
            var folderTarget = await ShowOpenFolderDialog.Handle(Unit.Default);
            // TODO: non-recursive
            if (Directory.Exists(folderSource))
            {
                foreach (var file in Directory.GetFiles(folderSource))
                {
                    var bitmap = new Bitmap(CVSharpGetEdge(file).ToMemoryStream());
                    var fileName = Path.GetFileName(file);
                    var targetFilePath = Path.Combine(folderTarget, fileName);
                    bitmap.Save(targetFilePath);
                }
            }
        }

        // OpenCVSharp
        private Mat CVSharpGetEdge(string path)
        {
            // Modifiable parameters
            var image = Cv2.ImRead(path);
            Cv2.CvtColor(image, image, ColorConversionCodes.BGR2GRAY);
            Cv2.AdaptiveThreshold(image,
                image,
                255,
                AdaptiveThresholdTypes.MeanC,
                ThresholdTypes.Binary,
                blockSize: 5,
                c: 7
            );
            return image;
        }
    }
}