using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Controls;
using MH.UI.Android.Extensions;
using MH.UI.Controls;
using MH.Utils.BaseClasses;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Utils;

public static class TreeMenuFactory {
  public static PopupWindow CreateMenu(Context context, TreeView dataContext, View anchor) {
    var treeMenu = new TreeMenuHost(context, dataContext);
    treeMenu.SetBackgroundResource(Resource.Drawable.view_border);
    treeMenu.SetPadding(DisplayU.DpToPx(1));

    var popup = new PopupWindow(treeMenu, ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent, true);

    var observer = new TreeMenuAdapterDataObserver(context, treeMenu, popup, anchor);
    treeMenu.Adapter!.RegisterAdapterDataObserver(observer);

    return popup;
  }

  private static int _getTreeMenuWidth(Resources res, Context context, IEnumerable<FlatTreeItem> items) {
    var generalPadding = res.GetDimensionPixelSize(Resource.Dimension.general_padding);

    var textView = new TextView(context);
    var paint = new Paint { TextSize = textView.TextSize };

    float maxTextWidth = 0;
    var maxLevel = 0;
    foreach (var item in items) {
      maxLevel = Math.Max(maxLevel, item.Level);
      if (item.TreeItem is MenuItem menuItem && !string.IsNullOrEmpty(menuItem.Text))
        maxTextWidth = Math.Max(maxTextWidth, paint.MeasureText(menuItem.Text));
    }

    var padding = DisplayU.DpToPx(2);
    var indent = res.GetDimensionPixelSize(Resource.Dimension.flat_tree_item_indent_size) * maxLevel;
    var icon = res.GetDimensionPixelSize(Resource.Dimension.icon_size) + (generalPadding * 2);
    var textPadding = generalPadding * 2;
    var expander = res.GetDimensionPixelSize(Resource.Dimension.icon_button_size);

    return (int)(padding + indent + icon + textPadding + maxTextWidth + expander);
  }

  private static int _getTreeMenuHeight(View anchor) {
    var location = new int[2];
    anchor.GetLocationOnScreen(location);

    var anchorBottom = location[1] + anchor.Height;
    var screenHeight = DisplayU.Metrics.HeightPixels;

    return screenHeight - anchorBottom;
  }

  private class TreeMenuAdapterDataObserver(Context context, TreeMenuHost treeMenu, PopupWindow popup, View anchor)
    : RecyclerView.AdapterDataObserver {

    public override void OnChanged() {
      base.OnChanged();
      UpdatePopupSize();
    }

    public void UpdatePopupSize() {
      var totalWidth = _getTreeMenuWidth(context.Resources!, context, treeMenu.Adapter!.Items);
      var totalHeight = treeMenu.Adapter!.ItemCount * context.Resources!.GetDimensionPixelSize(Resource.Dimension.menu_item_height);
      var maxWidth = DisplayU.Metrics.WidthPixels;
      var maxHeight = _getTreeMenuHeight(anchor);

      popup.Update(Math.Min(totalWidth, maxWidth), Math.Min(totalHeight, maxHeight));
    }
  }
}