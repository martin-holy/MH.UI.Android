using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using AndroidX.Core.Content;
using MH.Utils.Extensions;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Utils;

public static class Icons {
  private static readonly int[][] _tintStates = [
    [global::Android.Resource.Attribute.StateEnabled],
    [-global::Android.Resource.Attribute.StateEnabled]
  ];

  public static Color DefaultColor { get; set; } = Color.Gray;
  public static Dictionary<object, object>? IconNameToColor { get; set; }
  public static Func<string, string> ConvertIconName { get; set; } = (iconName) => iconName.ToSnakeCase();

  public static Drawable? GetDrawable(Context? context, string? iconName) {
    if (context?.Resources == null || iconName == null) return null;
    var id = context.Resources.GetIdentifier(ConvertIconName(iconName), "drawable", context.PackageName);
    if (id == 0) return null;
    return ContextCompat.GetDrawable(context, id);
  }

  public static Drawable? GetIcon(Context? context, string? iconName, int? colorId = null) =>
    _getIcon(context, iconName, colorId, null);

  public static Drawable? GetIcon(Context? context, string? iconName, Dictionary<object, object> iconNameToColor) =>
    _getIcon(context, iconName, null, iconNameToColor);

  public static Drawable? GetIcon(Context? context, int drawableId, int? colorId = null) =>
    _getIcon(context, drawableId, colorId, null);

  public static Drawable? GetIcon(Context? context, int drawableId, Dictionary<object, object> iconNameToColor) =>
    _getIcon(context, drawableId, null, iconNameToColor);

  private static Drawable? _getIcon(Context? context, string? iconName, int? colorId, Dictionary<object, object>? iconNameToColor) {
    if (GetDrawable(context, iconName) is not { } drawable) return null;
    var color = colorId != null
      ? ContextCompat.GetColor(context, (int)colorId)
      : GetColor(context, iconName!, iconNameToColor ?? IconNameToColor);
    return _applyColor(context!, drawable, color);
  }

  private static Drawable? _getIcon(Context? context, int drawableId, int? colorId, Dictionary<object, object>? iconNameToColor) {
    if (ContextCompat.GetDrawable(context, drawableId) is not { } drawable) return null;
    var color = colorId != null
      ? ContextCompat.GetColor(context, (int)colorId)
      : GetColor(context, "default", iconNameToColor ?? IconNameToColor);
    return _applyColor(context!, drawable, color);
  }

  private static Drawable? _applyColor(Context context, Drawable drawable, int color) {
    var colors = new int[] {
      color,
      ContextCompat.GetColor(context, Resource.Color.c_disabled_fo)
    };

    drawable.Mutate();
    drawable.SetTintList(new ColorStateList(_tintStates, colors));
    return drawable;
  }

  public static int GetColor(Context? context, string iconName, Dictionary<object, object>? iconNameToColor) {
    if (iconNameToColor == null
      || (!iconNameToColor.TryGetValue(iconName, out var colorId)
      && !iconNameToColor.TryGetValue("default", out colorId)))
      return DefaultColor;

    return ContextCompat.GetColor(context, (int)colorId);
  }
}