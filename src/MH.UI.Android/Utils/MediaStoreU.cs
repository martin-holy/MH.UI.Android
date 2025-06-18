using Android.Content;
using Android.Graphics;
using Android.Provider;

namespace MH.UI.Android.Utils;

public static class MediaStoreU {
  public static Bitmap? GetThumbnailBitmap(string imagePath, Context context) {
    var imageId = GetImageId(imagePath, context);
    if (imageId == -1) return null;

    return MediaStore.Images.Thumbnails.GetThumbnail(
      context.ContentResolver, imageId, ThumbnailKind.MiniKind, new() { InSampleSize = 1 });
  }

  public static long GetImageId(string filePath, Context context) {
    if (MediaStore.Images.Media.ExternalContentUri is not { } uri) return -1;

    var cursor = context.ContentResolver?.Query(
      uri,
      [MediaStore.Images.Media.InterfaceConsts.Id],
      $"{MediaStore.Images.Media.InterfaceConsts.Data}=?",
      [filePath],
      null);

    if (cursor?.MoveToFirst() != true) {
      cursor?.Close();
      return -1;
    }

    var id = cursor.GetLong(0);
    cursor.Close();

    return id;
  }
}