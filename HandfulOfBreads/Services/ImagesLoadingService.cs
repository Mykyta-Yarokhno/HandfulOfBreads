using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HandfulOfBreads.Views.Popups.ChoosePhotoPopup;

namespace HandfulOfBreads.Services
{
    public class ImagesLoadingService
    {
        public async Task<List<GalleryImage>> GetRecentImagesAsync(int count = 20)
        {
#if ANDROID
            var images = new List<GalleryImage>();

            var uri = Android.Provider.MediaStore.Images.Media.ExternalContentUri;
            string[] projection = { Android.Provider.MediaStore.Images.ImageColumns.Data, Android.Provider.MediaStore.Images.ImageColumns.DateAdded };
            string sortOrder = $"{Android.Provider.MediaStore.Images.ImageColumns.DateAdded} DESC";

            var cursor = Android.App.Application.Context.ContentResolver.Query(uri, projection, null, null, sortOrder);

            if (cursor?.MoveToFirst() == true)
            {
                int pathIndex = cursor.GetColumnIndex(Android.Provider.MediaStore.Images.ImageColumns.Data);

                while (!cursor.IsAfterLast && images.Count < count)
                {
                    var path = cursor.GetString(pathIndex);
                    if (File.Exists(path))
                    {
                        images.Add(new GalleryImage { FullPath = path });
                    }
                    cursor.MoveToNext();
                }
            }

            cursor?.Close();
            return images;
#else
    return new List<GalleryImage>();
#endif
        }
    }
}
