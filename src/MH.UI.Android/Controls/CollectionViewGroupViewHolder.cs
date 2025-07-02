using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls;

public class CollectionViewGroupViewHolder : RecyclerView.ViewHolder {
  private readonly LinearLayout _container;
  private readonly ImageView _expandedIcon;
  private readonly ImageView _icon;
  private readonly TextView _name;

  public CollectionViewGroupViewHolder(View itemView) : base(itemView) {
    _container = (LinearLayout)itemView;

    _expandedIcon = itemView.FindViewById<ImageView>(Resource.Id.expanded_icon)!;
    _expandedIcon.Click += _onExpandedChanged;

    _icon = itemView.FindViewById<ImageView>(Resource.Id.icon)!;
    _name = itemView.FindViewById<TextView>(Resource.Id.name)!;
  }

  public FlatTreeItem? Item { get; private set; }

  public void Bind(FlatTreeItem? item) {
    Item = item;
    if (item == null) return;

    int indent = item.Level * _container.Resources?.GetDimensionPixelSize(Resource.Dimension.flat_tree_item_indent_size) ?? 32;
    _container.SetPadding(indent, _container.PaddingTop, _container.PaddingRight, _container.PaddingBottom);

    _expandedIcon.Visibility = item.TreeItem.Items.Count > 0 ? ViewStates.Visible : ViewStates.Invisible;
    _expandedIcon.Selected = item.TreeItem.IsExpanded;

    _icon.SetImageDrawable(Icons.GetIcon(_container.Context, item.TreeItem.Icon));

    _name.SetText(item.TreeItem.Name, TextView.BufferType.Normal);
  }

  private void _onExpandedChanged(object? sender, System.EventArgs e) {
    if (Item == null) return;
    Item.TreeItem.IsExpanded = !Item.TreeItem.IsExpanded;
  }
}