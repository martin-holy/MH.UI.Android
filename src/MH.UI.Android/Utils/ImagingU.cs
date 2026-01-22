using Android.Graphics;
using Android.Media;
using Android.OS;
using MH.UI.Android.Extensions;
using MH.Utils;
using MH.Utils.Extensions;
using System;
using System.IO;

namespace MH.UI.Android.Utils;

public static class ImagingU {
  public static Bitmap? CreateImageThumbnail(string srcPath, int desiredSize) {
    var opts = new BitmapFactory.Options { InJustDecodeBounds = true };
    BitmapFactory.DecodeFile(srcPath, opts);
    var max = Math.Max(opts.OutWidth, opts.OutHeight);
    if (max <= 0) throw new BadImageFormatException();

    var decodeOpts = new BitmapFactory.Options {
      InSampleSize = _calculateSampleSize(max, desiredSize),
      InPreferredConfig = Bitmap.Config.Rgb565
    };
    return BitmapFactory.DecodeFile(srcPath, decodeOpts);
  }

  public static Bitmap? CreateImageRegionThumbnail(string srcPath, int x, int y, int size, int desiredSize) {
    using var decoder = Build.VERSION.SdkInt < BuildVersionCodes.S
      ? BitmapRegionDecoder.NewInstance(srcPath, false)
      : BitmapRegionDecoder.NewInstance(srcPath)
      ?? throw new Exception("Failed to create region decoder.");

    var decodeOpts = new BitmapFactory.Options {
      InSampleSize = _calculateSampleSize(size, desiredSize),
      InPreferredConfig = Bitmap.Config.Rgb565
    };
    var region = new Rect(x, y, x + size, y + size);
    if (decoder.DecodeRegion(region, decodeOpts) is not { } cropped) return null;

    var resized = Bitmap.CreateScaledBitmap(cropped, desiredSize, desiredSize, true);
    cropped.Dispose();

    return resized;
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

  public static object[]? GetVideoMetadata(string dirPath, string fileName) {
    var srcPath = IOExtensions.PathCombine(dirPath, fileName);
    var retriever = new MediaMetadataRetriever();
    try {
      retriever.SetDataSource(srcPath);

      int.TryParse(retriever.ExtractMetadata(MetadataKey.VideoHeight), out var h);
      int.TryParse(retriever.ExtractMetadata(MetadataKey.VideoWidth), out var w);
      int.TryParse(retriever.ExtractMetadata(MetadataKey.VideoRotation), out var o);
      retriever.ExtractMetadata(MetadataKey.CaptureFramerate).TryParseDoubleUniversal(out var fps);

      return [h, w, o, fps];
    }
    catch (Exception ex) {
      MH.Utils.Log.Error(ex);
      return null;
    }
    finally {
      retriever.Release();
    }
  }

  public static Bitmap ApplyOrientation(this Bitmap bitmap, MH.Utils.Imaging.Orientation orientation) {
    if (orientation == MH.Utils.Imaging.Orientation.Normal) return bitmap;

    var matrix = new Matrix();

    switch (orientation) {
      case MH.Utils.Imaging.Orientation.Rotate90:
        matrix.PostRotate(90);
        break;
      case MH.Utils.Imaging.Orientation.Rotate180:
        matrix.PostRotate(180);
        break;
      case MH.Utils.Imaging.Orientation.Rotate270:
        matrix.PostRotate(270);
        break;
      case MH.Utils.Imaging.Orientation.FlipHorizontal:
        matrix.PostScale(-1, 1);
        break;
      case MH.Utils.Imaging.Orientation.FlipVertical:
        matrix.PostScale(1, -1);
        break;
      case MH.Utils.Imaging.Orientation.Transpose:
        matrix.PostRotate(90);
        matrix.PostScale(-1, 1);
        break;
      case MH.Utils.Imaging.Orientation.Transverse:
        matrix.PostRotate(270);
        matrix.PostScale(-1, 1);
        break;
    }

    if (!matrix.IsIdentity) {
      var rotated = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
      bitmap.Recycle();
      bitmap = rotated;
    }

    return bitmap;
  }

  public static MH.Utils.Imaging.Orientation ConvertOrientationFromAndroidToMH(int orientation) =>
    orientation switch {
      (int)global::Android.Media.Orientation.Normal => MH.Utils.Imaging.Orientation.Normal,
      (int)global::Android.Media.Orientation.Rotate90 => MH.Utils.Imaging.Orientation.Rotate90,
      (int)global::Android.Media.Orientation.Rotate180 => MH.Utils.Imaging.Orientation.Rotate180,
      (int)global::Android.Media.Orientation.Rotate270 => MH.Utils.Imaging.Orientation.Rotate270,
      (int)global::Android.Media.Orientation.FlipHorizontal => MH.Utils.Imaging.Orientation.FlipHorizontal,
      (int)global::Android.Media.Orientation.FlipVertical => MH.Utils.Imaging.Orientation.FlipVertical,
      (int)global::Android.Media.Orientation.Transpose => MH.Utils.Imaging.Orientation.Transpose,
      (int)global::Android.Media.Orientation.Transverse => MH.Utils.Imaging.Orientation.Transverse,
      _ => MH.Utils.Imaging.Orientation.Normal
    };

  public static global::Android.Media.Orientation ConvertOrientationFromMHToAndroid(MH.Utils.Imaging.Orientation orientation) =>
    orientation switch {
      MH.Utils.Imaging.Orientation.Normal => global::Android.Media.Orientation.Normal,
      MH.Utils.Imaging.Orientation.Rotate90 => global::Android.Media.Orientation.Rotate90,
      MH.Utils.Imaging.Orientation.Rotate180 => global::Android.Media.Orientation.Rotate180,
      MH.Utils.Imaging.Orientation.Rotate270 => global::Android.Media.Orientation.Rotate270,
      MH.Utils.Imaging.Orientation.FlipHorizontal => global::Android.Media.Orientation.FlipHorizontal,
      MH.Utils.Imaging.Orientation.FlipVertical => global::Android.Media.Orientation.FlipVertical,
      MH.Utils.Imaging.Orientation.Transpose => global::Android.Media.Orientation.Transpose,
      MH.Utils.Imaging.Orientation.Transverse => global::Android.Media.Orientation.Transverse,
      _ => global::Android.Media.Orientation.Normal
    };

  private static int _calculateSampleSize(int srcSize, int desiredSize) {
    int sample = 1;
    while (srcSize / (sample * 2) >= desiredSize)
      sample *= 2;
    return sample;
  }

  public static void ResizeJpg(string src, string dest, int px, bool withMetadata, int quality) {
    using var srcBitmap = BitmapFactory.DecodeFile(src);
    if (srcBitmap == null) return;

    var (dstW, dstH) = _calculateSizeForTargetArea(srcBitmap.Width, srcBitmap.Height, px);

    using var resized = Bitmap.CreateScaledBitmap(srcBitmap, dstW, dstH, true);

    using (var fs = File.Open(dest, FileMode.Create, FileAccess.Write)) {
      resized.Compress(Bitmap.CompressFormat.Jpeg, quality, fs);
    }

    if (!withMetadata) return;

    var srcExif = new ExifInterface(src);
    var dstExif = new ExifInterface(dest);
    srcExif.CopyAttributes(dstExif);
    dstExif.UpdateDimensions(dstW, dstH);
    dstExif.SaveAttributes();

    var xmp = XmpU.ReadFromJpeg(src);
    if (!string.IsNullOrEmpty(xmp)) {
      xmp = XmpU.UpdateDimensions(xmp, dstW, dstH);
      XmpU.WriteToJpeg(dest, xmp);
    }
  }

  private static (int width, int height) _calculateSizeForTargetArea(int srcWidth, int srcHeight, int targetPixels) {
    int gcd = MathU.GreatestCommonDivisor(srcWidth, srcHeight);
    int rw = srcWidth / gcd;
    int rh = srcHeight / gcd;
    double q = Math.Sqrt((double)targetPixels / (rw * rh));
    int w = (int)Math.Round(q * rw);
    int h = (int)Math.Round(q * rh);

    return (w, h);
  }
}