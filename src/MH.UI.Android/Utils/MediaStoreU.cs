using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;

namespace MH.UI.Android.Utils;

public static class MediaStoreU {
  public static Bitmap? GetThumbnailBitmap(string imagePath, Context context) {
    var imageId = GetImageId(imagePath, context);

    if (Build.VERSION.SdkInt >= BuildVersionCodes.Q) {
      try {
        // Try to resolve MediaStore content:// URI fallback to file:// URI (slower, but works)
        var uri = imageId != -1
          ? ContentUris.WithAppendedId(MediaStore.Images.Media.ExternalContentUri!, imageId)
          : global::Android.Net.Uri.FromFile(new Java.IO.File(imagePath));
        var size = new global::Android.Util.Size(512, 512);
        return context.ContentResolver?.LoadThumbnail(uri, size, null);
      }
      catch { }
    }
    else {
      // --- Legacy API (before 29) ---
      if (imageId != -1) {
        try {
          return MediaStore.Images.Thumbnails.GetThumbnail(
            context.ContentResolver!, imageId, ThumbnailKind.MiniKind, new() { InSampleSize = 1 });
        }
        catch { }
      }
    }

    return GetThumbnailBitmapFromCustomCache(imagePath, context);
  }

  // TODO Fallback to custom cache
  public static Bitmap? GetThumbnailBitmapFromCustomCache(string filePath, Context context) {
    return null;
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