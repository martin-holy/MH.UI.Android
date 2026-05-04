using Android.Content;
using Android.Views;
using System;

namespace MH.UI.Android.Controls;

public class WrapLayout(Context context) : ViewGroup(context) {
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

      if (child.LayoutParameters is MarginLayoutParams mlp) {
        cw += mlp.LeftMargin + mlp.RightMargin;
        ch += mlp.TopMargin + mlp.BottomMargin;
      }

      if (lineWidth + cw > maxWidth && lineWidth > 0) {
        totalHeight += lineHeight;
        lineWidth = 0;
        lineHeight = 0;
      }

      lineWidth += cw;
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

      int ml = 0, mt = 0, mr = 0, mb = 0;

      if (child.LayoutParameters is MarginLayoutParams mlp) {
        ml = mlp.LeftMargin;
        mt = mlp.TopMargin;
        mr = mlp.RightMargin;
        mb = mlp.BottomMargin;
      }

      if (x + ml + cw + mr > maxWidth && x > PaddingLeft) {
        x = PaddingLeft;
        y += lineHeight;
        lineHeight = 0;
      }

      int cl = x + ml;
      int ct = y + mt;

      child.Layout(cl, ct, cl + cw, ct + ch);

      x += cw + ml + mr;
      lineHeight = Math.Max(lineHeight, ch + mt + mb);
    }
  }
}