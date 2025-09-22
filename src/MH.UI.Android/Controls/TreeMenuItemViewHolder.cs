using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;
using System;

namespace MH.UI.Android.Controls;

public class TreeMenuItemViewHolder : RecyclerView.ViewHolder {
  protected readonly LinearLayout _container;
  private readonly ImageView _expandedIcon;
  private readonly IconView _icon;
  private readonly TextView _name;
  protected bool _disposed;
  private readonly Action _closePopup;

  public FlatTreeItem? DataContext { get; private set; }

  public TreeMenuItemViewHolder(Context context, Action closePopup) : base(_createContainerView(context)) {
    _closePopup = closePopup;
    _icon = new IconView(context);

    _name = new TextView(context);
    _name.SetSingleLine(true);

    _expandedIcon = new ImageView(context) { Focusable = true };
    _expandedIcon.SetScaleType(ImageView.ScaleType.Center);
    _expandedIcon.SetImageResource(Resource.Drawable.tree_item_expanded_selector);

    _container = (LinearLayout)ItemView;
    _container.AddView(_icon, new LinearLayout.LayoutParams(DimensU.IconSize, DimensU.IconSize)
      .WithMargin(DimensU.Spacing, 0, DimensU.Spacing, 0));
    _container.AddView(_name, new LinearLayout.LayoutParams(0, LPU.Wrap, 1f));
    _container.AddView(_expandedIcon, new LinearLayout.LayoutParams(DimensU.IconButtonSize, DimensU.IconButtonSize));
    _container.Click += _onContainerClick;
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _container.Click -= _onContainerClick;
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  public void Bind(FlatTreeItem? item) {
    DataContext = item;
    if (item == null || item.TreeItem is not MenuItem menuItem) return;

    int indent = item.Level * DimensU.FlatTreeItemIndentSize;
    ItemView.SetPadding(indent, ItemView.PaddingTop, ItemView.PaddingRight, ItemView.PaddingBottom);

    ItemView.Enabled =
      menuItem.Items.Count > 0 ||
      menuItem.Command?.CanExecute(menuItem.CommandParameter) == true;

    _icon.Enabled = ItemView.Enabled;
    _icon.Bind(menuItem.Icon);

    _name.Text = menuItem.Text;
    _name.Enabled = ItemView.Enabled;

    _expandedIcon.Visibility = menuItem.Items.Count > 0 ? ViewStates.Visible : ViewStates.Invisible;
    _expandedIcon.Activated = menuItem.IsExpanded;
  }

  private void _onContainerClick(object? sender, EventArgs e) {
    if (DataContext?.TreeItem is not MenuItem item) return;

    if (item.Items.Count == 0) {
      if (item.Command?.CanExecute(item.CommandParameter) == true)
        item.Command.Execute(item.CommandParameter);

      _closePopup();

      return;
    }

    item.IsExpanded = !item.IsExpanded;
  }

  private static LinearLayout _createContainerView(Context context) {
    var container = new LinearLayout(context) {
      LayoutParameters = new ViewGroup.LayoutParams(LPU.Match, DimensU.MenuItemHeight),
      Orientation = Orientation.Horizontal,
      Clickable = true,
      Focusable = true
    };
    container.SetGravity(GravityFlags.CenterVertical);
    container.SetPadding(0);
    container.SetBackgroundResource(Resource.Color.c_static_ba);

    return container;
  }
}