using Android.Media;
using System;
using System.Globalization;

namespace MH.UI.Android.Extensions;

public static class ExifInterfaceExtensions {
  public static bool SetLatLong(this ExifInterface exif, double? lat, double? lng, ref bool changed) {
    var result = exif.SetLatLong(lat, lng);
    if (result) changed = true;
    return result;
  }

  public static bool SetLatLong(this ExifInterface exif, double? lat, double? lng) {
    var latLng = new float[2];
    if (exif.GetLatLong(latLng)) {
      if (lat == latLng[0] && lng == latLng[1])
        return false;
    }
    else if (lat == null || lng == null)
      return false;

    exif.SetAttribute(ExifInterface.TagGpsLatitude, lat == null ? null : _toExifDms(Math.Abs((double)lat)));
    exif.SetAttribute(ExifInterface.TagGpsLatitudeRef, lat == null ? null : (lat >= 0 ? "N" : "S"));
    exif.SetAttribute(ExifInterface.TagGpsLongitude, lng == null ? null : _toExifDms(Math.Abs((double)lng)));
    exif.SetAttribute(ExifInterface.TagGpsLongitudeRef, lng == null ? null : (lng >= 0 ? "E" : "W"));
    return true;
  }

  private static string _toExifDms(double value) {
    int deg = (int)value;
    value = (value - deg) * 60;
    int min = (int)value;
    double sec = (value - min) * 60;

    // EXIF rational: numerator/denominator
    return string.Format(
      CultureInfo.InvariantCulture,
      "{0}/1,{1}/1,{2}/1000",
      deg,
      min,
      (int)(sec * 1000)
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
    var existingComment = exif.GetAttribute(ExifInterface.TagUserComment);
    if (string.Equals(existingComment, comment)) return false;
    exif.SetAttribute(ExifInterface.TagUserComment, comment);
    return true;
  }
}