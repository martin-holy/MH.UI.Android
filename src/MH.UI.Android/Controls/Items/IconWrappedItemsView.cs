using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Utils;
using System;

namespace MH.UI.Android.Controls.Items;

public class IconWrappedItemsView : IconItemsViewBase {
  private readonly WrapLayout _wrapLayout;

  public int MaxHeight { get; set; } = int.MaxValue;
  public object[]? Items { get => _wrapLayout.Items; set => _wrapLayout.Items = value; }
  public int HorizontalSpacing { get => _wrapLayout.HorizontalSpacing; set => _wrapLayout.HorizontalSpacing = value; }
  public int VerticalSpacing { get => _wrapLayout.VerticalSpacing; set => _wrapLayout.VerticalSpacing = value; }

  public IconWrappedItemsView(Context context, string iconName, Func<object, View?> itemFactory) : base(context, iconName) {
    _wrapLayout = new WrapLayout(context, itemFactory);
    var scroll = new ScrollView(context) { FillViewport = true };
    scroll.AddView(_wrapLayout, LPU.LinearMatchWrap());
    AddView(scroll, LPU.Linear(0, LPU.Wrap, 1f));
  }

  protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) =>
    base.OnMeasure(widthMeasureSpec, MeasureSpec.MakeMeasureSpec(MaxHeight, MeasureSpecMode.AtMost));
}