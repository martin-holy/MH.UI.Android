using Android.Views;

namespace MH.UI.Android.Extensions;

public static class ViewGroupExtensions {
  public static TParent Add<TParent>(this TParent parent, View child, ViewGroup.LayoutParams lp)
    where TParent : ViewGroup {
    parent.AddView(child, lp);
    return parent;
  }
}