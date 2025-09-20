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
    var newLp = view.LayoutParameters is ViewGroup.MarginLayoutParams mlp
      ? new GridLayout.LayoutParams(rowSpec, columnSpec) {
        Width = mlp.Width,
        Height = mlp.Height,
        LeftMargin = mlp.LeftMargin,
        TopMargin = mlp.TopMargin,
        RightMargin = mlp.RightMargin,
        BottomMargin = mlp.BottomMargin
      }
      : new GridLayout.LayoutParams(rowSpec, columnSpec) {
        Width = view.LayoutParameters?.Width ?? ViewGroup.LayoutParams.WrapContent,
        Height = view.LayoutParameters?.Height ?? ViewGroup.LayoutParams.WrapContent
      };

    if (gravity != null) newLp.SetGravity((GravityFlags)gravity);
    view.LayoutParameters = newLp;

    return view;
  }
}