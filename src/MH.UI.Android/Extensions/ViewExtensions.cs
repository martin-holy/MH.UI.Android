using Android.Views;
using Android.Widget;
using System;

namespace MH.UI.Android.Extensions;

public static class ViewExtensions {
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

  public static T WithClickAction<T>(this T view, Action action) where T : View {
    var weakAction = new WeakReference<Action>(action);
    view.Click += (_, _) => {
      if (weakAction.TryGetTarget(out var act))
        act();
    };
    return view;
  }
}