using Android.Content;
using Android.Graphics.Drawables;
using Android.Widget;
using AndroidX.Core.Content;
using MH.UI.Android.Utils;

namespace MH.UI.Android.Controls;

public class IconView : ImageView {
  public IconView(Context context) : base(context) { }

  public IconView(Context context, string? iconName, int? colorId = null) : base(context) {
    _setDrawable(Icons.GetIcon(Context, iconName), colorId);
  }

  public IconView Bind(string? iconName, int? colorId = null) =>
    _setDrawable(Icons.GetIcon(Context, iconName), colorId);

  public IconView Bind(int? drawableId, int? colorId = null) =>
    _setDrawable(drawableId == null ? null : ContextCompat.GetDrawable(Context, (int)drawableId), colorId);

  private IconView _setDrawable(Drawable? drawable, int? colorId) {
    SetImageDrawable(drawable);
    return this;
  }
}