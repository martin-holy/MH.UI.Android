using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;
using System;

namespace MH.UI.Android.Controls;

public class FlatTreeItemViewHolder : RecyclerView.ViewHolder, IDisposable {
  private readonly TreeViewHost _treeViewHost;
  private readonly LinearLayout _container;
  private readonly ImageView _expandedIcon;
  private readonly ImageView _icon;
  private readonly TextView _name;
  private readonly CommandBinding _selectItemCommandBinding;
  private bool _disposed;  

  public FlatTreeItem? DataContext { get; private set; }

  public FlatTreeItemViewHolder(Context context, TreeViewHost treeViewHost) : base(_createContainerView(context)) {
    _treeViewHost = treeViewHost;
    _expandedIcon = ViewBuilder.CreateTreeItemExpandIconView(context);
    _expandedIcon.Click += _onExpandedChanged;
    _icon = new IconButton(context);
    _icon.Click += _onIconClick;
    _name = new TextView(context);
    _container = (LinearLayout)ItemView;
    _container.AddView(_expandedIcon);
    _container.AddView(_icon);
    _container.AddView(_name);
    _selectItemCommandBinding = new CommandBinding(_container, treeViewHost.DataContext.SelectItemCommand);
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _selectItemCommandBinding.Dispose();
      _expandedIcon.Click -= _onExpandedChanged;
      _icon.Click -= _onIconClick;
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  public void Bind(FlatTreeItem? item) {
    DataContext = item;
    _selectItemCommandBinding.Parameter = item?.TreeItem;
    if (item == null) return;

    int indent = item.Level * ItemView.Resources!.GetDimensionPixelSize(Resource.Dimension.flat_tree_item_indent_size);
    ItemView.SetPadding(indent, ItemView.PaddingTop, ItemView.PaddingRight, ItemView.PaddingBottom);

    _expandedIcon.Visibility = item.TreeItem.Items.Count > 0 ? ViewStates.Visible : ViewStates.Invisible;
    _expandedIcon.Selected = item.TreeItem.IsExpanded;

    _icon.SetImageDrawable(Icons.GetIcon(ItemView.Context, item.TreeItem.Icon));

    _name.SetText(item.TreeItem.Name, TextView.BufferType.Normal);
  }

  private void _onExpandedChanged(object? sender, EventArgs e) {
    if (DataContext == null) return;
    DataContext.TreeItem.IsExpanded = !DataContext.TreeItem.IsExpanded;
  }

  private void _onIconClick(object? sender, EventArgs e) {
    if (_treeViewHost.ItemMenuFactory(_icon, DataContext?.TreeItem) is not { } menu) return;
    menu.ShowAsDropDown(_icon);
  }

  private static LinearLayout _createContainerView(Context context) {
    var container = new LinearLayout(context) {
      LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
      Orientation = Orientation.Horizontal,
      Clickable = true,
      Focusable = true
    };
    container.SetGravity(GravityFlags.CenterVertical);
    container.SetPadding(context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding));
    container.SetBackgroundResource(Resource.Color.c_static_ba);

    return container;
  }
}