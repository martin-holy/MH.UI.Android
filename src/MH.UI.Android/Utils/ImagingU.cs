using Android.Graphics;
using System.IO;
using System;

namespace MH.UI.Android.Utils;
public static class ImagingU {
  public static void CreateImageThumbnail(string srcPath, string destPath, int desiredSize, int quality) {
    var opts = new BitmapFactory.Options { InJustDecodeBounds = true };
    BitmapFactory.DecodeFile(srcPath, opts);
    var max = Math.Max(opts.OutWidth, opts.OutHeight);
    if (max <= 0) throw new BadImageFormatException();

    var inSample = 1;    
    while (max / (inSample * 2) >= desiredSize) inSample *= 2;

    var decodeOpts = new BitmapFactory.Options { InSampleSize = inSample, InPreferredConfig = Bitmap.Config.Rgb565 };
    var bm = BitmapFactory.DecodeFile(srcPath, decodeOpts) ?? throw new BadImageFormatException();

    using var fs = new FileStream(destPath, FileMode.Create, FileAccess.Write);
    bm.Compress(Bitmap.CompressFormat.Jpeg, quality, fs);
  }
}