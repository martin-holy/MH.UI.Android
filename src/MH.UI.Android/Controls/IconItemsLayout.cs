using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using System;

namespace MH.UI.Android.Controls;

public class IconItemsLayout : LinearLayout {
  public WrapLayout WrapLayout { get; }

  public IconItemsLayout(Context context, string iconName, Func<object, View?> itemFactory) : base(context) {
    Orientation = Orientation.Horizontal;
    SetGravity(GravityFlags.CenterVertical);

    AddView(new IconView(context, iconName), new LayoutParams(DimensU.IconSize, DimensU.IconSize)
      .WithMargin(DimensU.Spacing, 0, DimensU.Spacing, 0));

    WrapLayout = new WrapLayout(context, itemFactory);

    var scroll = new ScrollView(context) { FillViewport = true };
    scroll.AddView(WrapLayout, new LayoutParams(LPU.Match, LPU.Wrap));

    AddView(scroll, new LayoutParams(0, LPU.Wrap, 1f));
  }
}