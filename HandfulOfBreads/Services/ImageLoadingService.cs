using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace HandfulOfBreads.Services
{
    public class ImageLoadingService
    {
        private Dictionary<string, ImageModel> _imageCache = new Dictionary<string, ImageModel>();

        public async Task<List<ImageModel>> LoadSavedImagesAsync(int thumbnailSize)
        {
            List<ImageModel> images = new List<ImageModel>();

#if ANDROID
            var picturesPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
            var files = Directory.GetFiles(picturesPath, "pixel_grid_*.png");

            foreach (var filePath in files)
            {
                try
                {
                    if (_imageCache.TryGetValue(filePath, out var cachedImage))
                    {
                        System.Diagnostics.Debug.WriteLine($"Image loaded from cache: {filePath}");
                        images.Add(cachedImage);
                        continue;
                    }

                    var fileName = Path.GetFileName(filePath);
                    SKImageInfo sizeInfo;
                    using (var stream = File.OpenRead(filePath))
                    {
                        sizeInfo = SKBitmap.DecodeBounds(stream);
                        System.Diagnostics.Debug.WriteLine($"Image size: Width={sizeInfo.Width}, Height={sizeInfo.Height}");
                    }

                    int columns = sizeInfo.Width / thumbnailSize;
                    int rows = sizeInfo.Height / thumbnailSize;

                    SKBitmap thumbnail = null;
                    await Task.Run(() =>
                    {
                        using (var stream = File.OpenRead(filePath))
                        {
                            var originalBitmap = SKBitmap.Decode(stream);
                            thumbnail = originalBitmap.Resize(new SKImageInfo(100, 100), SKFilterQuality.Medium);
                        }
                    });

                    ImageSource thumbnailSource = ImageSource.FromStream(() =>
                    {
                        using (SKImage image = SKImage.FromBitmap(thumbnail))
                        using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                        {
                            var memoryStream = new MemoryStream(data.ToArray());
                            System.Diagnostics.Debug.WriteLine($"MemoryStream length: {memoryStream.Length}");
                            return memoryStream;
                        }
                    });

                    var imageModel = new ImageModel
                    {
                        FilePath = filePath,
                        Thumbnail = thumbnailSource,
                        FileName = fileName,
                        Rows = rows,
                        Columns = columns
                    };

                    _imageCache[filePath] = imageModel;
                    images.Add(imageModel);
                    System.Diagnostics.Debug.WriteLine($"Image added to cache: {filePath}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading image {filePath}: {ex.Message}");
                }
            }
#else
            Console.WriteLine("Loading images is only supported on Android.");
#endif

            return images;
        }
    }

    public class ImageModel
    {
        public string FilePath { get; set; }
        public ImageSource Thumbnail { get; set; }
        public string FileName { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
    }
}