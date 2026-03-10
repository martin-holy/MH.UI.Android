using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;
using System;

namespace MH.UI.Android.Controls.Hosts.TreeMenuHost;

public class TreeMenuItemV : LinearLayout, IBindable<FlatTreeItem> {
  private readonly ImageView _expandedIcon;
  private readonly IconView _icon;
  private readonly TextView _name;
  private readonly Action _closePopup;

  public FlatTreeItem? DataContext { get; private set; }

  public TreeMenuItemV(Context context, Action closePopup) : base(context) {
    Orientation = Orientation.Horizontal;
    Clickable = true;
    Focusable = true;
    SetGravity(GravityFlags.CenterVertical);
    this.SetPadding(0);
    SetBackgroundResource(Resource.Color.c_static_ba);

    _closePopup = closePopup;
    _icon = new IconView(context);

    _name = new TextView(context);
    _name.SetSingleLine(true);

    _expandedIcon = new ImageView(context) { Focusable = true };
    _expandedIcon.SetScaleType(ImageView.ScaleType.Center);
    _expandedIcon.SetImageResource(Resource.Drawable.tree_item_expanded_selector);

    AddView(_icon, new LayoutParams(DimensU.IconSize, DimensU.IconSize).WithMargin(DimensU.Spacing, 0, DimensU.Spacing, 0));
    AddView(_name, new LayoutParams(0, LPU.Wrap, 1f));
    AddView(_expandedIcon, new LayoutParams(DimensU.IconButtonSize, DimensU.IconButtonSize));
    this.WithClickAction(_onContainerClick);
  }

  public void Bind(FlatTreeItem? item) {
    DataContext = item;
    if (item == null || item.TreeItem is not MenuItem menuItem) return;

    LayoutParameters!.Height = DimensU.MenuItemHeight;

    int indent = item.Level * DimensU.FlatTreeItemIndentSize;
    SetPadding(indent, PaddingTop, PaddingRight, PaddingBottom);

    _icon.Bind(menuItem.Icon);
    _name.Text = menuItem.Text;

    var enabled = menuItem.Items.Count > 0
      || menuItem.Command?.CanExecute(menuItem.CommandParameter) == true;

    Enabled = enabled;
    _icon.Enabled = enabled;
    _name.Enabled = enabled;

    BindIsExpandedVisible(item);
    BindIsExpanded(item);
  }

  public void BindIsExpanded(FlatTreeItem item) {
    _expandedIcon.Activated = item.TreeItem.IsExpanded;
  }

  public void BindIsExpandedVisible(FlatTreeItem item) {
    _expandedIcon.Visibility = item.TreeItem.Items.Count > 0 ? ViewStates.Visible : ViewStates.Invisible;
  }

  public void Unbind() { }

  private static void _onContainerClick(TreeMenuItemV o) {
    if (o.DataContext?.TreeItem is not MenuItem item) return;

    if (item.Items.Count == 0) {
      if (item.Command?.CanExecute(item.CommandParameter) == true)
        item.Command.Execute(item.CommandParameter);

      o._closePopup();

      return;
    }

    item.IsExpanded = !item.IsExpanded;
  }
}