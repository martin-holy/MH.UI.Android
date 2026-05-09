using Android.Graphics;
using Android.OS;
using System;
using System.IO;

namespace MH.UI.Android.Extensions;

public static class BitmapExtensions {
  public static Bitmap? Create(string srcPath, int x, int y, int width, int height, int inSampleSize = 1) =>
    Create(srcPath, new Rect(x, y, x + width, y + height), inSampleSize);

  public static Bitmap? Create(string srcPath, Rect region, int inSampleSize = 1) {
    using var decoder = Build.VERSION.SdkInt < BuildVersionCodes.S
      ? BitmapRegionDecoder.NewInstance(srcPath, false)
      : BitmapRegionDecoder.NewInstance(srcPath)
        ?? throw new Exception("Failed to create region decoder.");

    var opts = new BitmapFactory.Options {
      InSampleSize = Math.Max(1, inSampleSize),
      InPreferredConfig = Bitmap.Config.Rgb565
    };

    return decoder.DecodeRegion(region, opts);
  }

  public static void SaveAsJpeg(this Bitmap bmp, string filePath, int quality) {
    using var fs = File.Open(filePath, FileMode.Create, FileAccess.Write);
    bmp.Compress(Bitmap.CompressFormat.Jpeg!, quality, fs);
  }

  public static Bitmap Resize(this Bitmap bitmap, int size) {
    var scale = (float)size / Math.Max(bitmap.Width, bitmap.Height);

    if (scale >= 1f) return bitmap;

    var targetWidth = (int)(bitmap.Width * scale);
    var targetHeight = (int)(bitmap.Height * scale);
    var resized = Bitmap.CreateScaledBitmap(bitmap, targetWidth, targetHeight, true);
    bitmap.Recycle();

    return resized;
  }
}