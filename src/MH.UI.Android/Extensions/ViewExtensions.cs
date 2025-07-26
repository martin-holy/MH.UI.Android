using Android.Views;

namespace MH.UI.Android.Extensions;

public static class ViewExtensions {
  public static void SetMargin(this View view, int left, int top, int right, int bottom) {
    var layoutParams = view.LayoutParameters as ViewGroup.MarginLayoutParams
      ?? new ViewGroup.MarginLayoutParams(view.LayoutParameters
        ?? new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
    layoutParams.SetMargins(left, top, right, bottom);
    view.LayoutParameters = layoutParams;
  }

  public static void SetMarginUnified(this View view, int value) =>
    view.SetMargin(value, value, value, value);
}