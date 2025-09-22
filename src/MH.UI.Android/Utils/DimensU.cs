using Android.Content;

namespace MH.UI.Android.Utils;

public static class DimensU {
  public static int FlatTreeItemIndentSize { get; private set; }
  public static int IconSize { get; private set; }
  public static int IconButtonSize { get; private set; }
  public static int MenuItemHeight { get; private set; }
  public static int Spacing { get; private set; }

  public static void Init(Context context) {
    var res = context.Resources!;
    FlatTreeItemIndentSize = res.GetDimensionPixelSize(Resource.Dimension.flat_tree_item_indent_size);
    IconSize = res.GetDimensionPixelSize(Resource.Dimension.icon_size);
    IconButtonSize = res.GetDimensionPixelSize(Resource.Dimension.icon_button_size);
    MenuItemHeight = res.GetDimensionPixelSize(Resource.Dimension.menu_item_height);
    Spacing = res.GetDimensionPixelSize(Resource.Dimension.spacing);
  }
}