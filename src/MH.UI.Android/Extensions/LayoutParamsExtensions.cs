using Android.Views;

namespace MH.UI.Android.Extensions;

public static class LayoutParamsExtensions {
  public static T WithMargin<T>(this T layoutParams, int left, int top, int right, int bottom)
    where T : ViewGroup.MarginLayoutParams {
    layoutParams.SetMargins(left, top, right, bottom);
    return layoutParams;
  }

  public static T WithMargin<T>(this T layoutParams, int all)
    where T : ViewGroup.MarginLayoutParams {
    layoutParams.SetMargins(all, all, all, all);
    return layoutParams;
  }
}