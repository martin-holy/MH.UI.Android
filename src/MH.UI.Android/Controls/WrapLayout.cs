using Android.Content;
using Android.Views;
using MH.UI.Android.Utils;
using System;

namespace MH.UI.Android.Controls;

public class WrapLayout : ViewGroup {
  private object[]? _items;

  public object[]? Items { get => _items; set { _items = value; _populateItems(); } }
  public Func<object, View?> ItemFactory { get; set; }
  public int HorizontalSpacing { get; set; } = 4;
  public int VerticalSpacing { get; set; } = 4;

  public WrapLayout(Context context, Func<object, View?> itemFactory) : base(context) {
    ItemFactory = itemFactory;
  }

  private void _populateItems() {
    RemoveAllViews();
    if (_items == null) return;

    foreach (var item in _items)
      if (ItemFactory(item) is { } view)
        AddView(view, new LayoutParams(LPU.Wrap, LPU.Wrap));
  }

  protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {
    var width = MeasureSpec.GetSize(widthMeasureSpec);
    var widthMode = MeasureSpec.GetMode(widthMeasureSpec);
    var heightMode = MeasureSpec.GetMode(heightMeasureSpec);

    int maxWidth = widthMode == MeasureSpecMode.Unspecified
      ? int.MaxValue
      : width - PaddingLeft - PaddingRight;

    int lineWidth = 0;
    int lineHeight = 0;
    int totalHeight = PaddingTop + PaddingBottom;

    for (int i = 0; i < ChildCount; i++) {
      var child = GetChildAt(i);
      if (child == null || child.Visibility == ViewStates.Gone) continue;

      MeasureChild(child, widthMeasureSpec, heightMeasureSpec);

      int cw = child.MeasuredWidth;
      int ch = child.MeasuredHeight;

      if (lineWidth + cw > maxWidth && lineWidth > 0) {
        totalHeight += lineHeight + VerticalSpacing;
        lineWidth = 0;
        lineHeight = 0;
      }

      lineWidth += cw + HorizontalSpacing;
      lineHeight = Math.Max(lineHeight, ch);
    }

    totalHeight += lineHeight;

    int measuredHeight = heightMode == MeasureSpecMode.Exactly
      ? MeasureSpec.GetSize(heightMeasureSpec)
      : totalHeight;

    SetMeasuredDimension(width, measuredHeight);
  }

  protected override void OnLayout(bool changed, int left, int top, int right, int bottom) {
    int maxWidth = right - left - PaddingRight;
    int x = PaddingLeft;
    int y = PaddingTop;
    int lineHeight = 0;

    for (int i = 0; i < ChildCount; i++) {
      var child = GetChildAt(i);
      if (child == null || child.Visibility == ViewStates.Gone) continue;

      int cw = child.MeasuredWidth;
      int ch = child.MeasuredHeight;

      if (x + cw > maxWidth && x > PaddingLeft) {
        x = PaddingLeft;
        y += lineHeight + VerticalSpacing;
        lineHeight = 0;
      }

      child.Layout(x, y, x + cw, y + ch);

      x += cw + HorizontalSpacing;
      lineHeight = Math.Max(lineHeight, ch);
    }
  }
}