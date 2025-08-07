using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.UI.Android.Extensions;
using MH.Utils.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MH.UI.Android.Utils;

public static class MenuFactory {
  public static PopupWindow CreateMenu(Context context, View parent, MenuItem root) {
    var listView = new ListView(context) {
      LayoutParameters = new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent),
      ScrollBarStyle = ScrollbarStyles.OutsideOverlay,
      Divider = null,
      DividerHeight = 0,
      Adapter = new MenuAdapter(context, [.. root.Items.Cast<MenuItem>()], parent)
    };
    listView.SetBackgroundResource(Resource.Drawable.view_border);
    listView.SetPadding(DisplayU.DpToPx(1));

    var popup = new PopupWindow(listView, DisplayU.DpToPx(200), ViewGroup.LayoutParams.WrapContent, true);

    listView.Post(() => {
      int maxWidth = 0;
      for (int i = 0; i < listView.Adapter.Count; i++) {
        if (listView.Adapter.GetView(i, null, listView) is not { } view) continue;
        view.Measure(
          View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified),
          View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
        maxWidth = Math.Max(maxWidth, view.MeasuredWidth);
      }

      maxWidth += DisplayU.DpToPx(8);
      maxWidth = Math.Min(Math.Max(maxWidth, popup.Width), DisplayU.Metrics.WidthPixels);

      popup.Update(maxWidth, popup.Height);
    });

    return popup;
  }
}

internal class MenuAdapter(Context context, List<MenuItem> _items, View _parent) : ArrayAdapter<MenuItem>(context, 0, _items) {
  private readonly Dictionary<int, WeakReference<PopupWindow>> _subMenus = [];

  public override View GetView(int position, View? convertView, ViewGroup parent) {
    var item = _items[position];

    if (convertView is MenuItemHost view)
      view.Click -= _onMenuItemClick;
    else
      view = new MenuItemHost(Context);

    view.Bind(item);
    view.Click += _onMenuItemClick;
    view.SubMenu = _subMenus.TryGetValue(item.GetHashCode(), out var weakRef) && weakRef.TryGetTarget(out var subMenu)
      ? subMenu
      : null;

    return view;
  }

  private void _onMenuItemClick(object? sender, EventArgs e) {
    if (sender is not MenuItemHost { DataContext: { } item } host) return;

    if (item.Items.Count == 0) {
      if (item.Command?.CanExecute(item.CommandParameter) == true)
        item.Command.Execute(item.CommandParameter);

      return;
    }

    if (!_subMenus.TryGetValue(item.GetHashCode(), out var weakRef) || !weakRef.TryGetTarget(out var subMenu)) {
      subMenu = MenuFactory.CreateMenu(Context!, _parent, item);
      _subMenus[item.GetHashCode()] = new WeakReference<PopupWindow>(subMenu);
      host.SubMenu = subMenu;
    }

    var location = new int[2];
    host.GetLocationOnScreen(location);
    int x = location[0] + DisplayU.DpToPx(40);
    int y = location[1] + Context.Resources!.GetDimensionPixelSize(Resource.Dimension.menu_item_height);
    subMenu.ShowAtLocation(_parent, GravityFlags.NoGravity, x, y);
  }

  protected override void Dispose(bool disposing) {
    if (disposing) {
      foreach (var kvp in _subMenus)
        if (kvp.Value.TryGetTarget(out var popup))
          popup.Dismiss();

      _subMenus.Clear();
    }
    base.Dispose(disposing);
  }
}