using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media.Imaging;
using OpenCvSharp;
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
        public static Interaction<Unit, string?> ShowOpenFileDialog { get; } = new();
        public static Interaction<Unit, string?> ShowSaveFileDialog { get; } = new();
        
        // Init
        public MainWindowViewModel()
        {
            OpenImageCommand = ReactiveCommand.CreateFromTask(OpenImageAsync);
            SaveImageCommand = ReactiveCommand.CreateFromTask(SaveImageAsync);
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