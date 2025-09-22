using Android.Content;
using Android.Graphics.Drawables;
using Android.Widget;
using AndroidX.Core.Content;
using MH.UI.Android.Utils;

namespace MH.UI.Android.Controls;

public class IconView(Context context) : ImageView(context) {
  public IconView Bind(string iconName, int? colorId = null) =>
    _setDrawable(Icons.GetIcon(Context, iconName), colorId);

  public IconView Bind(int drawableId, int? colorId = null) =>
    _setDrawable(ContextCompat.GetDrawable(Context, drawableId), colorId);

  private IconView _setDrawable(Drawable? drawable, int? colorId) {
    if (drawable != null) {
      drawable.Mutate();

      int tintColor = Enabled
        ? (colorId != null ? ContextCompat.GetColor(Context, (int)colorId) : 0)
        : ContextCompat.GetColor(Context, Resource.Color.c_disabled_fo);

      if (tintColor != 0) drawable.SetTint(tintColor);
    }

    SetImageDrawable(drawable);
    return this;
  }
}