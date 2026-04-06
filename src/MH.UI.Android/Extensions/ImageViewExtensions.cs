using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;

namespace MH.UI.Android.Extensions;

public static class ImageViewExtensions {
  public static void UpdateImageBitmap(this ImageView view, Bitmap? bitmap) {
    var oldBitmap = view.Drawable is BitmapDrawable bd ? bd.Bitmap : null;
    view.SetImageBitmap(bitmap);
    if (oldBitmap?.IsRecycled == false && bitmap != oldBitmap) oldBitmap.Recycle();
  }
}