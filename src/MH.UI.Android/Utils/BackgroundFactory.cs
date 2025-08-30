using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using AndroidX.Core.Content;
using System.Collections.Concurrent;

namespace MH.UI.Android.Utils;

public static class BackgroundFactory {
  private static readonly ConcurrentDictionary<string, GradientDrawable> _cache = new();

  public static GradientDrawable Create(int fillResId, int strokeColorResId, int strokeWidthResId, int cornerRadiusResId) {
    var key = $"{fillResId}|{strokeColorResId}|{strokeWidthResId}|{cornerRadiusResId}";

    return _cache.GetOrAdd(key, _ => {
      var fill = new Color(ContextCompat.GetColor(Application.Context, fillResId));
      var stroke = new Color(ContextCompat.GetColor(Application.Context, strokeColorResId));
      var strokeWidth = (int)Application.Context.Resources.GetDimension(strokeWidthResId);

      var shape = new GradientDrawable();
      shape.SetColor(fill);
      shape.SetStroke(strokeWidth, stroke);

      if (cornerRadiusResId > 0)
        shape.SetCornerRadius(Application.Context.Resources.GetDimension(cornerRadiusResId));

      return shape;
    });
  }

  public static GradientDrawable Dark() =>
    Create(
      Resource.Color.c_black5,
      Resource.Color.c_black,
      Resource.Dimension.border_stroke_width,
      -1);

  public static GradientDrawable RoundDark() =>
    Create(
      Resource.Color.c_black5,
      Resource.Color.c_black,
      Resource.Dimension.border_stroke_width,
      Resource.Dimension.border_corner_radius);

  public static GradientDrawable RoundDarker() =>
    Create(
      Resource.Color.c_black2,
      Resource.Color.c_black,
      Resource.Dimension.border_stroke_width,
      Resource.Dimension.border_corner_radius);
}