using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using System;

namespace MH.UI.Android.Controls.Items;

public abstract class IconItemsViewBase : LinearLayout {
  private object[]? _items;

  protected readonly Func<object, View?> _itemFactory;

  public object[]? Items { get => _items; set { _items = value; _populateItems(_items); } }

  public IconItemsViewBase(Context context, string iconName, Func<object, View?> itemFactory) : base(context) {
    Orientation = Orientation.Horizontal;
    SetGravity(GravityFlags.CenterVertical);

    _itemFactory = itemFactory;

    AddView(new IconView(context, iconName),
      LPU.Linear(DimensU.IconSize, DimensU.IconSize)
        .WithMargin(DimensU.Spacing, 0, DimensU.Spacing, 0));
  }

  protected abstract void _populateItems(object[]? items);
}