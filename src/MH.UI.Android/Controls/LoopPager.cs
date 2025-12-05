using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Utils;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls;

public class LoopPager : ViewGroup {
  private int _current = 0;
  private int _visibleIndex = 1;
  private float _downX;
  private int _scrollStartX;
  private int _pageWidth;
  private readonly OverScroller _scroller;
  private readonly int _touchSlop;
  private bool _userInputEnabled = true;
  readonly List<View> _pages;

  public bool UserInputEnabled {
    get => _userInputEnabled;
    set => _userInputEnabled = value;
  }

  public LoopPager(Context context, List<View> pages) : base(context) {
    _pages = pages;
    _scroller = new OverScroller(context);
    _touchSlop = ViewConfiguration.Get(context).ScaledTouchSlop;
    ChildrenDrawingOrderEnabled = true;
    _reorderChildren();
  }

  protected override void OnSizeChanged(int w, int h, int oldw, int oldh) {
    base.OnSizeChanged(w, h, oldw, oldh);
    _pageWidth = w;
    _reorderChildren();
  }

  public void SetCurrentItem(int item, bool smoothScroll) {
    int count = _pages.Count;
    if (count == 0) return;

    if (item < 0)
      item = count - 1;
    else if (item >= count)
      item = 0;

    if (item == _current) return;

    int oldScroll = ScrollX;
    _current = item;
    _reorderChildren();

    if (!smoothScroll) {
      ScrollTo(_visibleIndex * _pageWidth, 0);
    }
    else {
      _scroller.StartScroll(oldScroll, 0, _visibleIndex * _pageWidth - oldScroll, 0, 300);
      PostInvalidateOnAnimation();
    }
  }

  protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {
    int w = MeasureSpec.GetSize(widthMeasureSpec);
    int h = MeasureSpec.GetSize(heightMeasureSpec);

    for (int i = 0; i < ChildCount; i++) {
      GetChildAt(i)!.Measure(
        MeasureSpec.MakeMeasureSpec(w, MeasureSpecMode.Exactly),
        MeasureSpec.MakeMeasureSpec(h, MeasureSpecMode.Exactly)
      );
    }

    SetMeasuredDimension(w, h);
  }

  protected override void OnLayout(bool changed, int l, int t, int r, int b) {
    for (int i = 0; i < ChildCount; i++) {
      int left = i * _pageWidth;
      GetChildAt(i)!.Layout(left, 0, left + _pageWidth, MeasuredHeight);
    }
    ScrollTo(_visibleIndex * _pageWidth, 0);
  }

  public override bool OnInterceptTouchEvent(MotionEvent? ev) {
    if (ev == null || !_userInputEnabled) return false;

    switch (ev.ActionMasked) {
      case MotionEventActions.Down:
        _downX = ev.GetX();
        _scrollStartX = ScrollX;
        return false;
      case MotionEventActions.Move:
        float dx = Math.Abs(ev.GetX() - _downX);
        if (dx > _touchSlop)
          return true;
        break;
    }

    return false;
  }

  public override bool OnTouchEvent(MotionEvent? ev) {
    if (ev == null || !_userInputEnabled) return false;

    switch (ev.ActionMasked) {
      case MotionEventActions.Down:
        _downX = ev.GetX();
        return true;

      case MotionEventActions.Move:
        float dx = _downX - ev.GetX();
        ScrollTo(_scrollStartX + (int)dx, 0);
        return true;

      case MotionEventActions.Up:
      case MotionEventActions.Cancel:
        _finishSwipe();
        return true;
    }

    return false;
  }

  private void _finishSwipe() {
    int w = MeasuredWidth;
    float pageOffset = (float)ScrollX / w;

    int target = _current;

    if (pageOffset > _visibleIndex + 0.3f)
      target = (_current + 1) % _pages.Count;
    else if (pageOffset < _visibleIndex - 0.3f)
      target = (_current - 1 + _pages.Count) % _pages.Count;

    _current = target;
    _reorderChildren();
  }

  private void _reorderChildren() {
    int count = _pages.Count;
    if (count == 0) return;

    RemoveAllViews();

    if (count == 1) {
      AddView(_pages[_current], new LayoutParams(LPU.Match, LPU.Match));
      _visibleIndex = 0;
    }
    else if (count == 2) {
      int right = (_current + 1) % count;
      AddView(_pages[_current], new LayoutParams(LPU.Match, LPU.Match));
      AddView(_pages[right], new LayoutParams(LPU.Match, LPU.Match));
      _visibleIndex = 0;
    }
    else {
      int left = (_current - 1 + count) % count;
      int right = (_current + 1) % count;
      AddView(_pages[left], new LayoutParams(LPU.Match, LPU.Match));
      AddView(_pages[_current], new LayoutParams(LPU.Match, LPU.Match));
      AddView(_pages[right], new LayoutParams(LPU.Match, LPU.Match));
      _visibleIndex = 1;
    }

    // Force measure and layout so all children render
    Measure(
      MeasureSpec.MakeMeasureSpec(MeasuredWidth, MeasureSpecMode.Exactly),
      MeasureSpec.MakeMeasureSpec(MeasuredHeight, MeasureSpecMode.Exactly)
    );

    for (int i = 0; i < ChildCount; i++) {
      int left = i * MeasuredWidth;
      GetChildAt(i)!.Layout(left, 0, left + MeasuredWidth, MeasuredHeight);
    }

    ScrollTo(_visibleIndex * _pageWidth, 0);
  }

  public override void ComputeScroll() {
    if (_scroller.ComputeScrollOffset()) {
      ScrollTo(_scroller.CurrX, _scroller.CurrY);
      PostInvalidateOnAnimation();
    }
  }

  protected override int GetChildDrawingOrder(int childCount, int i) {
    return i;
  }
}