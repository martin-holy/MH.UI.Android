using Android.Views;

namespace MH.UI.Android.Extensions;

public static class ViewExtensions {
  public static T SetMargin<T>(this T view, int left, int top, int right, int bottom) where T : View {
    var layoutParams = view.LayoutParameters as ViewGroup.MarginLayoutParams
      ?? new ViewGroup.MarginLayoutParams(view.LayoutParameters
        ?? new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
    layoutParams.SetMargins(left, top, right, bottom);
    view.LayoutParameters = layoutParams;
    return view;
  }

  public static void SetMargin(this ViewGroup.MarginLayoutParams layoutParams, int value) =>
    layoutParams.SetMargins(value, value, value, value);

  public static T SetMargin<T>(this T view, int value) where T : View =>
    view.SetMargin(value, value, value, value);

  public static void SetPadding(this View view, int value) =>
    view.SetPadding(value, value, value, value);
}