using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
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
    var generalPadding = context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding);

    _closePopup = closePopup;
    _icon = new IconView(context).SetMargin(generalPadding, 0, generalPadding, 0);
    _name = _createTextView(context);
    _expandedIcon = _createTreeItemExpandIconView(context);
    _container = (LinearLayout)ItemView;
    _container.AddView(_icon);
    _container.AddView(_name);
    _container.AddView(_expandedIcon);
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

    int indent = item.Level * ItemView.Resources!.GetDimensionPixelSize(Resource.Dimension.flat_tree_item_indent_size);
    ItemView.SetPadding(indent, ItemView.PaddingTop, ItemView.PaddingRight, ItemView.PaddingBottom);

    ItemView.Enabled =
      menuItem.Items.Count > 0 ||
      menuItem.Command?.CanExecute(menuItem.CommandParameter) == true;

    _icon.Enabled = ItemView.Enabled;
    _icon.Bind(menuItem.Icon);

    _name.SetText(menuItem.Text, TextView.BufferType.Normal);
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
      LayoutParameters = new ViewGroup.LayoutParams(
        ViewGroup.LayoutParams.MatchParent,
        context.Resources!.GetDimensionPixelSize(Resource.Dimension.menu_item_height)),
      Orientation = Orientation.Horizontal,
      Clickable = true,
      Focusable = true
    };
    container.SetGravity(GravityFlags.CenterVertical);
    container.SetPadding(0);
    container.SetBackgroundResource(Resource.Color.c_static_ba);

    return container;
  }

  private static TextView _createTextView(Context context) {
    var textView = new TextView(context) {
      LayoutParameters = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WrapContent, 1f)
    };
    textView.SetSingleLine(true);

    return textView;
  }

  private static ImageView _createTreeItemExpandIconView(Context context) {
    var size = context.Resources!.GetDimensionPixelSize(Resource.Dimension.icon_button_size);
    var icon = new ImageView(context) {
      Focusable = true,
      LayoutParameters = new LinearLayout.LayoutParams(size, size)
    };
    icon.SetScaleType(ImageView.ScaleType.Center);
    icon.SetImageResource(Resource.Drawable.tree_item_expanded_selector);

    return icon;
  }
}