using Android.Content;
using Android.Graphics.Drawables;
using Android.Widget;
using AndroidX.Core.Content;
using MH.UI.Android.Utils;

namespace MH.UI.Android.Controls;

public class IconView : ImageView {
  public IconView(Context context) : this(context, context.Resources!.GetDimensionPixelSize(Resource.Dimension.icon_size)) { }

  public IconView(Context context, int size) : base(context) {
    LayoutParameters = new LinearLayout.LayoutParams(size, size);
  }

  public IconView Bind(string iconName, int? colorId = null) =>
    _setDrawable(Icons.GetIcon(Context, iconName), colorId);

  public IconView Bind(int drawableId, int? colorId = null) =>
    _setDrawable(ContextCompat.GetDrawable(Context, drawableId), colorId);

  private IconView _setDrawable(Drawable? drawable, int? colorId) {
    if (drawable != null && colorId != null) {
      drawable.Mutate();
      drawable.SetTint(ContextCompat.GetColor(Context, (int)colorId));
    }

    SetImageDrawable(drawable);
    return this;
  }
}