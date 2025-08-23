using Android.Views;
using Android.Widget;

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

  public static bool SetVisibleIf(this View view, bool value, ViewStates ifNot = ViewStates.Gone) {
    if (value) {
      view.Visibility = ViewStates.Visible;
      return true;
    }

    view.Visibility = ifNot;
    return false;
  }

  public static T SetGridPosition<T>(this T view, GridLayout.Spec? rowSpec, GridLayout.Spec? columnSpec, GravityFlags? gravity = null) where T : View {
    int width = view.LayoutParameters?.Width ?? ViewGroup.LayoutParams.WrapContent;
    int height = view.LayoutParameters?.Height ?? ViewGroup.LayoutParams.WrapContent;

    int left = 0, top = 0, right = 0, bottom = 0;
    if (view.LayoutParameters is ViewGroup.MarginLayoutParams mlp) {
      left = mlp.LeftMargin;
      top = mlp.TopMargin;
      right = mlp.RightMargin;
      bottom = mlp.BottomMargin;
    }

    var lp = new GridLayout.LayoutParams(rowSpec, columnSpec) {
      Width = width,
      Height = height,
      LeftMargin = left,
      TopMargin = top,
      RightMargin = right,
      BottomMargin = bottom
    };

    if (gravity != null) lp.SetGravity((GravityFlags)gravity);
    view.LayoutParameters = lp;

    return view;
  }
}