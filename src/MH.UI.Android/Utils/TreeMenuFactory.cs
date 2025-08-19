using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.UI.Android.Extensions;
using MH.UI.Controls;
using MH.Utils.BaseClasses;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Utils;

public static class TreeMenuFactory {
  public static PopupWindow CreateMenu(Context context, TreeView dataContext) {
    var treeMenu = new TreeMenuHost(context, dataContext);
    treeMenu.SetBackgroundResource(Resource.Drawable.view_border);
    treeMenu.SetPadding(DisplayU.DpToPx(1));

    var popup = new PopupWindow(treeMenu, ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent, true);

    treeMenu.Post(() => {
      var totalHeight = treeMenu.Adapter!.ItemCount * context.Resources!.GetDimensionPixelSize(Resource.Dimension.menu_item_height);
      var maxHeight = DisplayU.Metrics.HeightPixels;
      var totalWidth = _getTreeMenuWidth(context.Resources!, context, treeMenu.Adapter.Items);
      var screenWidth = DisplayU.Metrics.WidthPixels;

      popup.Update(Math.Min(totalWidth, screenWidth), Math.Min(totalHeight, maxHeight));
    });

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
}