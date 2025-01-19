using HandfulOfBreads.Graphics.DrawablePatterns;

namespace HandfulOfBreads.Services
{
    internal class ImageSavingService
    {
        public async Task SaveImageToGalleryAsync(IPatternDrawable _drawable)
        {
            #if ANDROID
                        var fileName = $"pixel_grid_{DateTime.Now:yyyyMMdd_HHmmss}.png";

                        var picturesPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
                        var filePath = Path.Combine(picturesPath, fileName);

                        await _drawable.SaveToFileAsync(filePath);

                        AddImageToGallery(filePath);
            #else
                        Console.WriteLine("Saving to gallery is only supported on Android.");
            #endif
        }

        #if ANDROID
                private void AddImageToGallery(string filePath)
                {
                    var context = Android.App.Application.Context;

                    Android.Media.MediaScannerConnection.ScanFile(
                        context,
                        new[] { filePath },
                        null,
                        null
                    );
                }
        #endif
    }
}
