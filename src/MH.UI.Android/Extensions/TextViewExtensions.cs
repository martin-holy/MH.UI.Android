using Android.Graphics;
using Android.Widget;
using AndroidX.Core.Content;

namespace MH.UI.Android.Extensions;

public static class TextViewExtensions {
  public static TextView WithTextColor(this TextView textView, int colorResourceId) {
    textView.SetTextColor(new Color(ContextCompat.GetColor(textView.Context, (int)colorResourceId)));
    return textView;
  }
}