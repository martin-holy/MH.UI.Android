using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls;

public class TreeMenuHostSizeObserver(Context _context, TreeMenuHost _treeMenu, PopupWindow _popup) : RecyclerView.AdapterDataObserver {
  private int _lastItemsCount;

  public View? MenuAnchor { get; set; }

  public override void OnChanged() {
    base.OnChanged();

    var itemsCount = _treeMenu.Adapter!.Items.Count;
    if (_lastItemsCount != itemsCount) {
      _lastItemsCount = itemsCount;
      UpdatePopupSize();
    }
  }

  public void UpdatePopupSize() {
    if (MenuAnchor == null) throw new ArgumentNullException(nameof(MenuAnchor));
    var minHeight = DimensU.MenuItemHeight * 5;
    var totalWidth = _getTreeMenuWidth(_context, _treeMenu.Adapter!.Items);
    var totalHeight = _treeMenu.Adapter!.ItemCount * DimensU.MenuItemHeight;
    var maxWidth = DisplayU.Metrics.WidthPixels;
    var maxHeight = _getTreeMenuHeight(MenuAnchor, _popup);
    var targetWidth = Math.Min(totalWidth, maxWidth);
    var targetHeight = Math.Min(totalHeight, maxHeight >= minHeight ? maxHeight : minHeight);

    if (targetWidth >= _treeMenu.Width && targetHeight >= _treeMenu.Height) {
      _updateTreeMenuSize(LPU.Match, LPU.Match);
      _updatePopupSize(targetWidth, targetHeight);
    }
    else {
      _updateTreeMenuSize(targetWidth, targetHeight);
      _treeMenu.Post(() => _updatePopupSize(targetWidth, targetHeight));
    }
  }

  private void _updatePopupSize(int width, int height) {
    if (_popup.IsShowing)
      _popup.Update(width, height);
    else {
      _popup.Width = width;
      _popup.Height = height;
    }
  }

  private void _updateTreeMenuSize(int width, int height) {
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

  private static int _getTreeMenuWidth(Context context, IEnumerable<FlatTreeItem> items) {
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
    var indent = DimensU.FlatTreeItemIndentSize * maxLevel;
    var icon = DimensU.IconSize + DimensU.Spacing * 2;
    var textPadding = DimensU.Spacing * 2;
    var expander = DimensU.IconButtonSize;

    return (int)(padding + indent + icon + textPadding + maxTextWidth + expander);
  }

  private static int _getTreeMenuHeight(View anchor, PopupWindow popup) {
    var anchorLoc = new int[2];
    anchor.GetLocationOnScreen(anchorLoc);
    var anchorTop = anchorLoc[1];
    var anchorBottom = anchorTop + anchor.Height;

    var popupLoc = new int[2];
    popup.ContentView.GetLocationOnScreen(popupLoc);
    var popupY = popupLoc[1];

    return popupY == 0
      ? Math.Max(anchorTop, DisplayU.Metrics.HeightPixels - anchorBottom)
      : DisplayU.Metrics.HeightPixels - popupY;
  }
}