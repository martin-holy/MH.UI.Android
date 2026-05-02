using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;

namespace MH.UI.Android.Controls.Items;

public abstract class IconItemsViewBase : LinearLayout {
  public IconItemsViewBase(Context context, string iconName) : base(context) {
    Orientation = Orientation.Horizontal;
    SetGravity(GravityFlags.CenterVertical);

    AddView(new IconView(context, iconName),
      LPU.Linear(DimensU.IconSize, DimensU.IconSize)
        .WithMargin(DimensU.Spacing, 0, DimensU.Spacing, 0));
  }
}