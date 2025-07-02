using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls;

public class FlatTreeItemViewHolder : RecyclerView.ViewHolder {
  private readonly TreeView _vmParent;
  private readonly ImageView _expandedIcon;
  private readonly ImageView _icon;
  private readonly TextView _name;

  public FlatTreeItem? DataContext { get; private set; }

  public FlatTreeItemViewHolder(View itemView, TreeView vmParent) : base(itemView) {
    _vmParent = vmParent;

    itemView.Click += _onContainerClick;

    _expandedIcon = itemView.FindViewById<ImageView>(Resource.Id.expanded_icon)!;
    _expandedIcon.Click += _onExpandedChanged;

    _icon = itemView.FindViewById<ImageView>(Resource.Id.icon)!;
    _name = itemView.FindViewById<TextView>(Resource.Id.name)!;
  }

  public void Bind(FlatTreeItem? item) {
    DataContext = item;
    if (item == null) return;

    int indent = item.Level * ItemView.Resources?.GetDimensionPixelSize(Resource.Dimension.flat_tree_item_indent_size) ?? 32;
    ItemView.SetPadding(indent, ItemView.PaddingTop, ItemView.PaddingRight, ItemView.PaddingBottom);

    _expandedIcon.Visibility = item.TreeItem.Items.Count > 0 ? ViewStates.Visible : ViewStates.Invisible;
    _expandedIcon.Selected = item.TreeItem.IsExpanded;

    _icon.SetImageDrawable(Icons.GetIcon(ItemView.Context, item.TreeItem.Icon));

    _name.SetText(item.TreeItem.Name, TextView.BufferType.Normal);
  }

  public static FlatTreeItemViewHolder Create(ViewGroup parent, TreeView vmParent) =>
    new(LayoutInflater.From(parent.Context)!.Inflate(Resource.Layout.flat_tree_item, parent, false)!, vmParent);

  private void _onExpandedChanged(object? sender, System.EventArgs e) {
    if (DataContext == null) return;
    DataContext.TreeItem.IsExpanded = !DataContext.TreeItem.IsExpanded;
  }

  private void _onContainerClick(object? sender, System.EventArgs e) {
    if (DataContext != null && _vmParent.SelectItemCommand.CanExecute(DataContext.TreeItem))
      _vmParent.SelectItemCommand.Execute(DataContext.TreeItem);
  }
}