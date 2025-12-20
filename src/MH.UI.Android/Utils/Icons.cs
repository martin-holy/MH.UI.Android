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
    if (context == null || context.Resources == null || iconName == null) return null;
    var id = context.Resources.GetIdentifier(ConvertIconName(iconName), "drawable", context.PackageName);
    if (id == 0) return null;
    return ContextCompat.GetDrawable(context, id);
  }

  public static Drawable? GetIcon(Context? context, string? iconName, Dictionary<object, object>? iconNameToColor = null) {
    if (context == null || iconName == null) return null;
    if (GetDrawable(context, iconName) is not { } drawable) return null;

    var colors = new int[] {
      GetColor(context, iconName, iconNameToColor ?? IconNameToColor),
      ContextCompat.GetColor(context, Resource.Color.c_disabled_fo)
    };

    drawable.Mutate();
    drawable.SetTintList(new ColorStateList(_tintStates, colors));
    return drawable;
  }

  public static int GetColor(Context context, string iconName, Dictionary<object, object>? iconNameToColor) {
    if (iconNameToColor == null
      || (!iconNameToColor.TryGetValue(iconName, out var colorId)
      && !iconNameToColor.TryGetValue("default", out colorId)))
      return DefaultColor;

    return ContextCompat.GetColor(context, (int)colorId);
  }
}