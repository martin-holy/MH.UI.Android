using Android.Views;
using Android.Widget;
using MH.UI.Android.Utils;

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

  public static T WithDpMargin<T>(this T layoutParams, int left, int top, int right, int bottom)
    where T : ViewGroup.MarginLayoutParams {
    layoutParams.SetMargins(DisplayU.DpToPx(left), DisplayU.DpToPx(top), DisplayU.DpToPx(right), DisplayU.DpToPx(bottom));
    return layoutParams;
  }

  public static T WithDpMargin<T>(this T layoutParams, int all)
    where T : ViewGroup.MarginLayoutParams {
    var px = DisplayU.DpToPx(all);
    layoutParams.SetMargins(px, px, px, px);
    return layoutParams;
  }

  public static RelativeLayout.LayoutParams WithRule(this RelativeLayout.LayoutParams layoutParams, LayoutRules verb) {
    layoutParams.AddRule(verb);
    return layoutParams;
  }

  public static RelativeLayout.LayoutParams WithRule(this RelativeLayout.LayoutParams layoutParams, LayoutRules verb, int subject) {
    layoutParams.AddRule(verb, subject);
    return layoutParams;
  }
}