using Android.Media;
using System;
using System.Globalization;

namespace MH.UI.Android.Extensions;

public static class ExifInterfaceExtensions {
  private const string _userCommentAsciiPrefix = "ASCII\0\0\0";

  public static bool SetLatLong(this ExifInterface exif, double? lat, double? lng, ref bool changed) {
    var result = exif.SetLatLong(lat, lng);
    if (result) changed = true;
    return result;
  }

  public static bool SetLatLong(this ExifInterface exif, double? lat, double? lng) {
    var latLng = new float[2];
    if (exif.GetLatLong(latLng)) {
      if (_almostEqual(lat, latLng[0]) && _almostEqual(lng, latLng[1]))
        return false;
    }
    else if (lat == null || lng == null)
      return false;

    if (lat == null || lng == null) {
      exif.SetAttribute(ExifInterface.TagGpsLatitude, null);
      exif.SetAttribute(ExifInterface.TagGpsLatitudeRef, null);
      exif.SetAttribute(ExifInterface.TagGpsLongitude, null);
      exif.SetAttribute(ExifInterface.TagGpsLongitudeRef, null);
      exif.SetAttribute(ExifInterface.TagGpsAltitude, null);
      exif.SetAttribute(ExifInterface.TagGpsAltitudeRef, null);
      exif.SetAttribute(ExifInterface.TagGpsTimestamp, null);
      exif.SetAttribute(ExifInterface.TagGpsDatestamp, null);
      return true;
    }

    exif.SetAttribute(ExifInterface.TagGpsLatitude, _toExifDms(Math.Abs((double)lat)));
    exif.SetAttribute(ExifInterface.TagGpsLatitudeRef, (lat >= 0 ? "N" : "S"));
    exif.SetAttribute(ExifInterface.TagGpsLongitude, _toExifDms(Math.Abs((double)lng)));
    exif.SetAttribute(ExifInterface.TagGpsLongitudeRef, (lng >= 0 ? "E" : "W"));
    return true;
  }

  private static bool _almostEqual(double? a, double? b, double eps = 1e-6) {
    if (a == null || b == null) return false;
    return Math.Abs(a.Value - b.Value) < eps;
  }

  private static string _toExifDms(double value) {
    value = Math.Abs(value);

    int deg = (int)Math.Floor(value);
    value = (value - deg) * 60.0;

    int min = (int)Math.Floor(value);
    double sec = (value - min) * 60.0;

    // Keep 4 decimal digits for seconds
    int secScaled = (int)Math.Round(sec * 10000);

    // Normalize overflow (rare but possible due to rounding)
    if (secScaled >= 60 * 10000) {
      secScaled = 0;
      min++;
      if (min >= 60) {
        min = 0;
        deg++;
      }
    }

    return string.Format(
      CultureInfo.InvariantCulture,
      "{0}/1,{1}/1,{2}/10000",
      deg,
      min,
      secScaled
    );
  }

  public static bool SetOrientation(this ExifInterface exif, Orientation orientation, ref bool changed) {
    var result = exif.SetOrientation(orientation);
    if (result) changed = true;
    return result;
  }

  public static bool SetOrientation(this ExifInterface exif, Orientation orientation) {
    var existingOrientation = exif.GetAttributeInt(ExifInterface.TagOrientation, (int)Orientation.Normal);
    if (existingOrientation == (int)orientation) return false;
    exif.SetAttribute(ExifInterface.TagOrientation, ((int)orientation).ToString());
    return true;
  }

  public static bool SetUserComment(this ExifInterface exif, string? comment, ref bool changed) {
    var result = exif.SetUserComment(comment);
    if (result) changed = true;
    return result;
  }

  public static bool SetUserComment(this ExifInterface exif, string? comment) {
    var existingComment = _stripUserCommentPrefix(exif.GetAttribute(ExifInterface.TagUserComment));

    if (string.Equals(existingComment, comment, StringComparison.Ordinal))
      return false;

    exif.SetAttribute(ExifInterface.TagUserComment,
      string.IsNullOrEmpty(comment)
        ? null
        : _userCommentAsciiPrefix + comment);

    return true;
  }

  private static string? _stripUserCommentPrefix(string? value) {
    if (!string.IsNullOrEmpty(value) && (
      value.StartsWith("ASCII\0\0\0", StringComparison.Ordinal) ||
      value.StartsWith("UNICODE\0", StringComparison.Ordinal)))
      return value.Substring(8);

    return value;
  }
}