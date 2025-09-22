using Android.Content;

namespace MH.UI.Android.Utils;

public static class DimensU {
  public static int IconSize { get; private set; }
  public static int IconButtonSize { get; private set; }
  public static int Spacing { get; private set; }

  public static void Init(Context context) {
    var res = context.Resources!;
    IconSize = res.GetDimensionPixelSize(Resource.Dimension.icon_size);
    IconButtonSize = res.GetDimensionPixelSize(Resource.Dimension.icon_button_size);
    Spacing = res.GetDimensionPixelSize(Resource.Dimension.spacing);
  }
}