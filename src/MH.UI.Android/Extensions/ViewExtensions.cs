using Android.Views;
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