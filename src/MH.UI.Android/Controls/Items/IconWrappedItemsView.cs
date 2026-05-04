using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls.Items;

public class IconWrappedItemsView : IconItemsViewBase {
  private readonly WrapLayout _wrapLayout;

  public int MaxHeight { get; set; } = int.MaxValue;
  public int Spacing { get; set; } = DimensU.CompactSpacing;

  public IconWrappedItemsView(Context context, string iconName, Func<object, View?> itemFactory) : base(context, iconName, itemFactory) {
    _wrapLayout = new WrapLayout(context);
    var scroll = new ScrollView(context) { FillViewport = true };
    scroll.AddView(_wrapLayout, LPU.LinearMatchWrap());
    AddView(scroll, LPU.Linear(0, LPU.Wrap, 1f));
  }

  protected override void _populateItems(IEnumerable<object>? items) {
    _wrapLayout.RemoveAllViews();
    if (items == null) return;

    int half = Spacing / 2;

    foreach (var item in items)
      if (_itemFactory(item) is { } view)
        _wrapLayout.AddView(view, LPU.ViewGroupWrap().WithMargin(half));

    _wrapLayout.SetPadding(half);
  }

  protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) =>
    base.OnMeasure(widthMeasureSpec, MeasureSpec.MakeMeasureSpec(MaxHeight, MeasureSpecMode.AtMost));
}