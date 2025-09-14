using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls;

public class TreeMenuHostSizeObserver(Context _context, TreeMenuHost _treeMenu, PopupWindow _popup, View _anchor) : RecyclerView.AdapterDataObserver {

  public override void OnChanged() {
    base.OnChanged();
    _updatePopupSize();
  }

  private void _updatePopupSize() {
    if (!_popup.IsShowing) return;
    var totalWidth = _getTreeMenuWidth(_context.Resources!, _context, _treeMenu.Adapter!.Items);
    var totalHeight = _treeMenu.Adapter!.ItemCount * _context.Resources!.GetDimensionPixelSize(Resource.Dimension.menu_item_height);
    var maxWidth = DisplayU.Metrics.WidthPixels;
    var maxHeight = _getTreeMenuHeight(_anchor);
    var targetWidth = Math.Min(totalWidth, maxWidth);
    var targetHeight = Math.Min(totalHeight, maxHeight);

    if (targetWidth >= _treeMenu.Width && targetHeight >= _treeMenu.Height) {
      _setTreeMenuSize(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
      _popup.Update(targetWidth, targetHeight);
    }
    else {
      _setTreeMenuSize(targetWidth, targetHeight);
      _treeMenu.Post(() => _popup.Update(targetWidth, targetHeight));
    }
  }

  private void _setTreeMenuSize(int width, int height) {
    var lp = _treeMenu.LayoutParameters;
    if (lp == null) {
      lp = new ViewGroup.LayoutParams(width, height);
    }
    else {
      lp.Width = width;
      lp.Height = height;
    }

    _treeMenu.LayoutParameters = lp;
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
    var icon = res.GetDimensionPixelSize(Resource.Dimension.icon_size) + generalPadding * 2;
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
}