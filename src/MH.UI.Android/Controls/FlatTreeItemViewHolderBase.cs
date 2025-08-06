using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;
using System;

namespace MH.UI.Android.Controls;

public abstract class FlatTreeItemViewHolderBase : RecyclerView.ViewHolder {
  private readonly IAndroidTreeViewHost _treeViewHost;
  protected readonly LinearLayout _container;
  private readonly ImageView _expandedIcon;
  private readonly ImageView _icon;
  private readonly TextView _name;
  protected bool _disposed;  

  public FlatTreeItem? DataContext { get; private set; }

  public FlatTreeItemViewHolderBase(Context context, IAndroidTreeViewHost treeViewHost) : base(_createContainerView(context)) {
    _treeViewHost = treeViewHost;
    _expandedIcon = _createTreeItemExpandIconView(context);
    _expandedIcon.Click += _onExpandedChanged;
    _icon = new IconButton(context);
    _icon.Click += _onIconClick;
    _name = new TextView(context);
    _container = (LinearLayout)ItemView;
    _container.AddView(_expandedIcon);
    _container.AddView(_icon);
    _container.AddView(_name);
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _expandedIcon.Click -= _onExpandedChanged;
      _icon.Click -= _onIconClick;
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  public virtual void Bind(FlatTreeItem? item) {
    DataContext = item;
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

  private static ImageView _createTreeItemExpandIconView(Context context) {
    var size = context.Resources!.GetDimensionPixelSize(Resource.Dimension.icon_button_size);
    var generalPadding = context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding);
    var icon = new ImageView(context) {
      Clickable = true,
      Focusable = true,
      LayoutParameters = new LinearLayout.LayoutParams(size, size) {
        MarginStart = generalPadding,
        MarginEnd = generalPadding
      }
    };
    icon.SetScaleType(ImageView.ScaleType.Center);
    icon.SetImageResource(Resource.Drawable.tree_item_expanded_selector);

    return icon;
  }
}