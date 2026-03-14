using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using System;

namespace MH.UI.Android.Controls;

public class IconItemsLayout : LinearLayout {
  public WrapLayout WrapLayout { get; }
  public int MaxHeight { get; set; } = int.MaxValue;

  public IconItemsLayout(Context context, string iconName, Func<object, View?> itemFactory) : base(context) {
    Orientation = Orientation.Horizontal;
    SetGravity(GravityFlags.CenterVertical);

    WrapLayout = new WrapLayout(context, itemFactory);
    var icon = new IconView(context, iconName);
    var scroll = new ScrollView(context) { FillViewport = true };

    scroll.AddView(WrapLayout, LPU.LinearMatchWrap());
    AddView(icon, LPU.Linear(DimensU.IconSize, DimensU.IconSize).WithMargin(DimensU.Spacing, 0, DimensU.Spacing, 0));
    AddView(scroll, LPU.Linear(0, LPU.Wrap, 1f));
  }

  protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) =>
    base.OnMeasure(widthMeasureSpec, MeasureSpec.MakeMeasureSpec(MaxHeight, MeasureSpecMode.AtMost));
}