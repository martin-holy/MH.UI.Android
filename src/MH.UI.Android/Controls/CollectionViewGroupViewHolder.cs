using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;
using System;

namespace MH.UI.Android.Controls;

public class CollectionViewGroupViewHolder : RecyclerView.ViewHolder {
  private readonly LinearLayout _container;
  private readonly ImageView _expandedIcon;
  private readonly ImageView _icon;
  private readonly TextView _name;
  private FlatTreeItem? _item;

  public CollectionViewGroupViewHolder(Context context) : base(_createContainerView(context)) {
    _expandedIcon = ViewBuilder.CreateTreeItemExpandIconView(context);
    _expandedIcon.Click += _onExpandedChanged;
    _icon = _createIconView(context);
    _name = new TextView(context);
    _container = (LinearLayout)ItemView;
    _container.AddView(_expandedIcon);
    _container.AddView(_icon);
    _container.AddView(_name);
  }

  public void Bind(FlatTreeItem item) {
    _item = item;

    int indent = item.Level * ItemView.Resources!.GetDimensionPixelSize(Resource.Dimension.flat_tree_item_indent_size);
    ItemView.SetPadding(indent, ItemView.PaddingTop, ItemView.PaddingRight, ItemView.PaddingBottom);

    _expandedIcon.Visibility = item.TreeItem.Items.Count > 0 ? ViewStates.Visible : ViewStates.Invisible;
    _expandedIcon.Selected = item.TreeItem.IsExpanded;

    _icon.SetImageDrawable(Icons.GetIcon(ItemView.Context, item.TreeItem.Icon));

    _name.SetText(item.TreeItem.Name, TextView.BufferType.Normal);
  }

  private void _onExpandedChanged(object? sender, EventArgs e) {
    if (_item == null) return;
    _item.TreeItem.IsExpanded = !_item.TreeItem.IsExpanded;
  }

  private static LinearLayout _createContainerView(Context context) {
    var container = new LinearLayout(context) {
      LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
      Orientation = Orientation.Horizontal
    };
    container.SetGravity(GravityFlags.CenterVertical);
    container.SetPadding(context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding));
    container.SetBackgroundResource(Resource.Color.gray1);

    return container;
  }

  private static ImageView _createIconView(Context context) =>
    new(context) {
      LayoutParameters = new LinearLayout.LayoutParams(DisplayU.DpToPx(24), DisplayU.DpToPx(24)) {
        MarginStart = DisplayU.DpToPx(8)
      }
    };
}