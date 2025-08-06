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
  public override View GetView(int position, View? convertView, ViewGroup parent) {
    var item = _items[position];
    var view = new MenuItemHost(Context, item);
    view.Click += _onMenuItemClick;

    return view;
  }

  private void _onMenuItemClick(object? sender, EventArgs e) {
    if (sender is not MenuItemHost { DataContext: { } item } host) return;

    if (item.Items.Count == 0) {
      if (item.Command?.CanExecute(item.CommandParameter) == true)
        item.Command.Execute(item.CommandParameter);

      return;
    }

    var subPopup = MenuFactory.CreateMenu(Context!, _parent, item);
    var location = new int[2];
    host.GetLocationOnScreen(location);
    int x = location[0] + DisplayU.DpToPx(40);
    int y = location[1] + Context.Resources!.GetDimensionPixelSize(Resource.Dimension.menu_item_height);
    subPopup.ShowAtLocation(_parent, GravityFlags.NoGravity, x, y);
  }
}