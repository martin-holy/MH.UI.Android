using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using System;

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

  protected override void _populateItems(object[]? items) {
    _container.RemoveAllViews();
    if (items == null) return;

    for (int i = 0; i < items.Length; i++) {
      if (_itemFactory(items[i]) is not { } view) continue;

      var lp = LPU.LinearWrap();

      if (i < items.Length - 1)
        lp.WithMargin(0, 0, Spacing, 0);

      _container.AddView(view, lp);
    }
  }
}