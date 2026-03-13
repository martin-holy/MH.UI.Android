using Android.Content;
using Android.Widget;

namespace MH.UI.Android.Utils;

public static class LayoutU {
  public static LinearLayout Horizontal(Context? context) =>
    new(context) { Orientation = Orientation.Horizontal };

  public static LinearLayout Vertical(Context? context) =>
    new(context) { Orientation = Orientation.Vertical };
}