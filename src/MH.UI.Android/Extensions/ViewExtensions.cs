using Android.OS;
using Android.Views;
using System;

namespace MH.UI.Android.Extensions;

public static class ViewExtensions {
  public static void SetPadding(this View view, int value) =>
    view.SetPadding(value, value, value, value);

  public static T WithPadding<T>(this T view, int value) where T : View {
    view.SetPadding(value);
    return view;
  }

  public static T WithPadding<T>(this T view, int left, int top, int right, int bottom) where T : View {
    view.SetPadding(left, top, right, bottom);
    return view;
  }

  public static bool SetVisibleIf(this View view, bool value, ViewStates ifNot = ViewStates.Gone) {
    if (value) {
      view.Visibility = ViewStates.Visible;
      return true;
    }

    view.Visibility = ifNot;
    return false;
  }

  public static T WithClickAction<T>(this T view, Action<T> action) where T : View {
    view.Click += Handler;
    void Handler(object? s, EventArgs _) => action((T)s!);
    return view;
  }

  public static void ApplySystemBarInsets(this View view) {
    if (Build.VERSION.SdkInt < BuildVersionCodes.R) return;
    view.SetOnApplyWindowInsetsListener(new InsetsListener(view.PaddingLeft, view.PaddingTop, view.PaddingRight, view.PaddingBottom));
    view.RequestApplyInsets();
  }

  private class InsetsListener(int left, int top, int right, int bottom) : Java.Lang.Object, View.IOnApplyWindowInsetsListener {
    public WindowInsets OnApplyWindowInsets(View v, WindowInsets insets) {
      var bars = insets.GetInsets(WindowInsets.Type.SystemBars());
      v.SetPadding(left + bars.Left, top + bars.Top, right + bars.Right, bottom + bars.Bottom);
      return insets;
    }
  }
}