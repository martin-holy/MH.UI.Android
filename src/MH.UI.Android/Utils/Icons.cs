using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using AndroidX.Core.Content;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MH.UI.Android.Utils;

public static class Icons {
  public static Color DefaultColor { get; set; } = Color.Gray;
  public static Dictionary<object, object>? IconNameToColor { get; set; }
  public static Func<string, string> ConvertIconName { get; set; } = (iconName) =>
    string.Concat(iconName.Select((c, i) => char.IsUpper(c) ? i == 0 ? c.ToString().ToLower() : "_" + char.ToLower(c) : c.ToString()));

  public static Drawable? GetIcon(Context? context, string? iconName, Dictionary<object, object>? iconNameToColor = null) {
    if (context == null || context.Resources == null || iconName == null) return null;
    var id = context.Resources.GetIdentifier(ConvertIconName(iconName), "drawable", context.PackageName);
    if (id == 0) return null;
    if (ContextCompat.GetDrawable(context, id) is not { } drawable) return null;
    drawable.Mutate();
    drawable.SetTint(GetColor(context, iconName, iconNameToColor ?? IconNameToColor));
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