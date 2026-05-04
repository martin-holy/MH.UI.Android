using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls.Items;

public class IconItemsView : IconItemsViewBase {
  private readonly LinearLayout _container;

  public int Spacing { get; set; } = DimensU.CompactSpacing;

  public IconItemsView(Context context, string iconName, Func<object, View?> itemFactory) : base(context, iconName, itemFactory) {
    _container = LayoutU.Horizontal(context);

    var scroll = new HorizontalScrollView(context) {
      HorizontalScrollBarEnabled = false,
      OverScrollMode = OverScrollMode.Never
    };

    scroll.AddView(_container, LPU.FrameWrap());

    AddView(scroll, LPU.Linear(0, LPU.Wrap, 1f));
  }

  protected override void _populateItems(IEnumerable<object>? items) {
    _container.RemoveAllViews();
    if (items == null) return;

    int half = Spacing / 2;

    foreach (var item in items)
      if (_itemFactory(item) is { } view)
        _container.AddView(view, LPU.LinearWrap().WithMargin(half));

    _container.SetPadding(half);
  }
}