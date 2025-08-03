using Android.Content;
using Android.Widget;

namespace MH.UI.Android.Controls;

public static class ViewBuilder {
  public static ImageView CreateTreeItemExpandIconView(Context context) {
    var size = context.Resources!.GetDimensionPixelSize(Resource.Dimension.icon_button_size);
    var generalPadding = context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding);
    var icon = new ImageView(context) {
      Clickable = true,
      Focusable = true,
      LayoutParameters = new LinearLayout.LayoutParams(size, size) {
        MarginStart = generalPadding,
        MarginEnd = generalPadding
      }
    };
    icon.SetScaleType(ImageView.ScaleType.Center);
    icon.SetImageResource(Resource.Drawable.tree_item_expanded_selector);

    return icon;
  }
}