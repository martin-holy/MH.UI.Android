using Android.Content;
using Android.Widget;
using MH.UI.Android.Utils;

namespace MH.UI.Android.Controls;

public class IconButton : ImageButton {
  public IconButton(Context context) : base(context) {
    var size = context.Resources!.GetDimensionPixelSize(Resource.Dimension.icon_button_size);
    LayoutParameters = new(size, size);
  }

  public void SetIcon(string iconName) =>
    SetImageDrawable(Icons.GetIcon(Context, iconName));
}