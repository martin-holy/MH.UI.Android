using Android.Graphics;
using System;

namespace MH.UI.Android.Utils;
public static class ImagingU {
  public static Bitmap? CreateImageThumbnail(string srcPath, int desiredSize) {
    var opts = new BitmapFactory.Options { InJustDecodeBounds = true };
    BitmapFactory.DecodeFile(srcPath, opts);
    var max = Math.Max(opts.OutWidth, opts.OutHeight);
    if (max <= 0) throw new BadImageFormatException();

    var inSample = 1;
    while (max / (inSample * 2) >= desiredSize) inSample *= 2;

    var decodeOpts = new BitmapFactory.Options { InSampleSize = inSample, InPreferredConfig = Bitmap.Config.Rgb565 };
    return BitmapFactory.DecodeFile(srcPath, decodeOpts);
  }
}