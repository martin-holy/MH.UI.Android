﻿using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls;

public class CollectionViewGroupViewHolder : RecyclerView.ViewHolder {
  private readonly ImageView _expandedIcon;
  private readonly ImageView _icon;
  private readonly TextView _name;

  public CollectionViewGroupViewHolder(View itemView) : base(itemView) {
    _expandedIcon = itemView.FindViewById<ImageView>(Resource.Id.expanded_icon)!;
    _expandedIcon.Click += _onExpandedChanged;

    _icon = itemView.FindViewById<ImageView>(Resource.Id.icon)!;
    _name = itemView.FindViewById<TextView>(Resource.Id.name)!;
  }

  public FlatTreeItem? Item { get; private set; }

  public void Bind(FlatTreeItem? item) {
    Item = item;
    if (item == null) return;

    int indent = item.Level * ItemView.Resources!.GetDimensionPixelSize(Resource.Dimension.flat_tree_item_indent_size);
    ItemView.SetPadding(indent, ItemView.PaddingTop, ItemView.PaddingRight, ItemView.PaddingBottom);

    _expandedIcon.Visibility = item.TreeItem.Items.Count > 0 ? ViewStates.Visible : ViewStates.Invisible;
    _expandedIcon.Selected = item.TreeItem.IsExpanded;

    _icon.SetImageDrawable(Icons.GetIcon(ItemView.Context, item.TreeItem.Icon));

    _name.SetText(item.TreeItem.Name, TextView.BufferType.Normal);
  }

  public static CollectionViewGroupViewHolder Create(ViewGroup parent) =>
    new(LayoutInflater.From(parent.Context)!.Inflate(Resource.Layout.collection_view_group, parent, false)!);

  private void _onExpandedChanged(object? sender, System.EventArgs e) {
    if (Item == null) return;
    Item.TreeItem.IsExpanded = !Item.TreeItem.IsExpanded;
  }
}