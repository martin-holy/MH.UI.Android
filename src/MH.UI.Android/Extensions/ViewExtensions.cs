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

  public static T WithClickAction<T, TOwner>(this T view, TOwner owner, Action<TOwner, T> action)
  where T : View
  where TOwner : class {
    var weakOwner = new WeakReference<TOwner>(owner);
    view.Click += (s, _) => {
      if (weakOwner.TryGetTarget(out var target))
        action(target, (T)s!);
    };
    return view;
  }
}