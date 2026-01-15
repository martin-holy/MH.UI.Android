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
}
