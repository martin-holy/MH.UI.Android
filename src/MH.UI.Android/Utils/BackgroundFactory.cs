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

    var bg = _cache.GetOrAdd(key, _ => {
      var fill = new Color(ContextCompat.GetColor(Application.Context, fillResId));
      var stroke = new Color(ContextCompat.GetColor(Application.Context, strokeColorResId));
      var strokeWidth = (int)Application.Context.Resources!.GetDimension(strokeWidthResId);

      var shape = new GradientDrawable();
      shape.SetColor(fill);
      shape.SetStroke(strokeWidth, stroke);

      if (cornerRadiusResId > 0)
        shape.SetCornerRadius(Application.Context.Resources.GetDimension(cornerRadiusResId));

      return (GradientDrawable)shape.Mutate();
    });

    var state = bg.GetConstantState();
    var clone = state != null ? state.NewDrawable() : bg;
    clone = clone.Mutate();

    return (GradientDrawable)clone;
  }

  public static Drawable Create(int fillResId, int strokeColorResId, int strokeWidthResId, int cornerRadiusResId, int shadowColorResId) {
    var res = Application.Context.Resources!;
    var fillColor = new Color(ContextCompat.GetColor(Application.Context, fillResId));
    var strokeColor = new Color(ContextCompat.GetColor(Application.Context, strokeColorResId));
    var strokeWidth = (int)res.GetDimension(strokeWidthResId);
    var cornerRadius = cornerRadiusResId > 0 ? res.GetDimension(cornerRadiusResId) : 0f;
    var shadowColor = new Color(ContextCompat.GetColor(Application.Context, shadowColorResId));

    var shadow = new GradientDrawable();
    shadow.SetColor(Color.Transparent);
    shadow.SetStroke(strokeWidth, shadowColor);
    if (cornerRadius > 0)
      shadow.SetCornerRadius(cornerRadius);

    var stroke = new GradientDrawable();
    stroke.SetColor(fillColor);
    stroke.SetStroke(strokeWidth, strokeColor);
    if (cornerRadius > 0)
      stroke.SetCornerRadius(cornerRadius);

    var drawable = new LayerDrawable([shadow, stroke]);
    drawable.SetLayerInset(0, strokeWidth, strokeWidth, 0, 0);
    drawable.SetLayerInset(1, 0, 0, strokeWidth, strokeWidth);

    return drawable;
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

  public static GradientDrawable RoundSolidDark() =>
    Create(
      Resource.Color.gray1,
      Resource.Color.c_black,
      Resource.Dimension.border_stroke_width,
      Resource.Dimension.border_corner_radius);
}