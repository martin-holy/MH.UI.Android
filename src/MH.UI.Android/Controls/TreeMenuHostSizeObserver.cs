using Android.Animation;
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

public class TreeMenuHostSizeObserver(Context context, TreeMenuHost treeMenu, PopupWindow popup, View anchor) : RecyclerView.AdapterDataObserver {

  public override void OnChanged() {
    base.OnChanged();
    UpdatePopupSize();
  }

  public void UpdatePopupSize() {
    var totalWidth = _getTreeMenuWidth(context.Resources!, context, treeMenu.Adapter!.Items);
    var totalHeight = treeMenu.Adapter!.ItemCount * context.Resources!.GetDimensionPixelSize(Resource.Dimension.menu_item_height);
    var maxWidth = DisplayU.Metrics.WidthPixels;
    var maxHeight = _getTreeMenuHeight(anchor);

    var targetWidth = Math.Min(totalWidth, maxWidth);
    var targetHeight = Math.Min(totalHeight, maxHeight);

    var currentWidth = treeMenu.Width > 0 ? treeMenu.Width : targetWidth;
    var currentHeight = treeMenu.Height > 0 ? treeMenu.Height : targetHeight;

    var lp = treeMenu.LayoutParameters;
    if (lp == null) {
      lp = new ViewGroup.LayoutParams(currentWidth, currentHeight);
      treeMenu.LayoutParameters = lp;
    }

    if (ValueAnimator.OfFloat(0f, 1f) is not { } animator) return;
    animator.SetDuration(150);
    animator.Update += (s, e) => {
      float fraction = (e.Animation.AnimatedValue as Java.Lang.Float)?.FloatValue() ?? 0f;
      lp.Width = (int)(currentWidth + (targetWidth - currentWidth) * fraction);
      lp.Height = (int)(currentHeight + (targetHeight - currentHeight) * fraction);
      treeMenu.LayoutParameters = lp;
      treeMenu.RequestLayout();
    };

    animator.AnimationEnd += (s, e) => {
      popup.Update(targetWidth, targetHeight);
    };

    animator.Start();
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