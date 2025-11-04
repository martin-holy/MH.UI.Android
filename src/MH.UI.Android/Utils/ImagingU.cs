using Android.Graphics;
using Android.Media;
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

  public static Bitmap? CreateVideoThumbnail(string srcPath, int desiredSize) {
    var retriever = new MediaMetadataRetriever();
    try {
      retriever.SetDataSource(srcPath);

      if (retriever.GetFrameAtTime(1_000_000, Option.ClosestSync) is not { } frame) return null;

      var width = frame.Width;
      var height = frame.Height;
      var scale = (float)desiredSize / Math.Max(width, height);
      if (scale >= 1f) return frame;

      var newW = (int)(width * scale);
      var newH = (int)(height * scale);
      var scaled = Bitmap.CreateScaledBitmap(frame, newW, newH, true);
      frame.Recycle();
      return scaled;
    }
    catch (Exception ex) {
      MH.Utils.Log.Error(ex);
      return null;
    }
    finally {
      retriever.Release();
    }
  }
}