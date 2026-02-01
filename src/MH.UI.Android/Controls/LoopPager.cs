using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager2.Widget;
using MH.UI.Android.Utils;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls;

public class LoopPager : ViewGroup {
  private int _visibleIndex = 0;
  private float _downX;
  private float _downY;
  private int _scrollStartX;
  private int _pageWidth;
  private bool _canIntercept;
  private bool _isDragging;
  private readonly OverScroller _scroller;
  private readonly int _touchSlop;
  readonly List<View> _pages;

  public bool UserInputEnabled { get; set; } = true;
  public bool AllowDragOnlyFromEdge { get; set; } = true;
  public Func<View, bool> IsHorizontalGestureConsumer { get; set; }
  public Func<View, bool> IsGestureBoundary { get; set; }
  public Func<View?, bool> IsPagerBlockedBy { get; set; }
  public int EdgeSwipeSizePx { get; set; }

  public LoopPager(Context context, List<View> pages) : base(context) {
    _pages = pages;
    _scroller = new OverScroller(context);
    _touchSlop = ViewConfiguration.Get(context)!.ScaledTouchSlop;
    EdgeSwipeSizePx = DisplayU.DpToPx(24);
    IsHorizontalGestureConsumer = DefaultIsHorizontalGestureConsumer;
    IsGestureBoundary = DefaultIsGestureBoundary;
    IsPagerBlockedBy = DefaultIsPagerBlockedBy;

    foreach (var page in _pages)
      AddView(page, new LayoutParams(LPU.Match, LPU.Match));

    _visibleIndex = 0;
    _reorderChildren();
    _scrollToVisibleIndex();
  }

  public int GetCurrentItem() =>
    _pages.IndexOf(GetChildAt(_visibleIndex)!);

  public void SetCurrentItem(int index, bool smoothScroll) {
    var newVisibleIndex = _indexToVisibleIndex(index);
    if (_visibleIndex == newVisibleIndex) return;

    int oldScroll = ScrollX;
    _visibleIndex = newVisibleIndex;
    _reorderChildren();

    if (smoothScroll) {
      _scroller.StartScroll(oldScroll, 0, _visibleIndex * _pageWidth - oldScroll, 0, 300);
      PostInvalidateOnAnimation();
    }
    else
      _scrollToVisibleIndex();
  }

  private void _scrollToVisibleIndex() =>
    ScrollTo(_visibleIndex * _pageWidth, 0);

  private int _indexToVisibleIndex(int index) {
    var page = _pages[index];

    for (int i = 0; i < ChildCount; i++)
      if (ReferenceEquals(page, GetChildAt(i)))
        return i;

    throw new IndexOutOfRangeException();
  }

  private void _reorderChildren() {
    int count = _pages.Count;
    if (count == 0) return;

    if (_visibleIndex > 0 && _visibleIndex < count - 1) return;

    if (_visibleIndex == 0 && GetChildAt(ChildCount - 1) is { } last) {
      RemoveViewAt(ChildCount - 1);
      AddView(last, 0);
      _visibleIndex++;
    }
    else if (_visibleIndex == count - 1 && GetChildAt(0) is { } first) {
      RemoveViewAt(0);
      AddView(first);
      _visibleIndex--;
    }
  }

  protected override void OnSizeChanged(int w, int h, int oldw, int oldh) {
    base.OnSizeChanged(w, h, oldw, oldh);
    _pageWidth = w;
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
    _scrollToVisibleIndex();
  }

  public static bool DefaultIsHorizontalGestureConsumer(View v) =>
    v is HorizontalScrollView ||
    v is SeekBar ||
    (v is ViewPager2 vp && vp.Orientation == (int)Orientation.Horizontal && vp.UserInputEnabled) ||
    (v as RecyclerView)?.GetLayoutManager()?.CanScrollHorizontally() == true;

  public static bool DefaultIsGestureBoundary(View v) =>
    v is ViewPager2 vp && vp.Orientation == (int)Orientation.Horizontal && !vp.UserInputEnabled;

  public static bool DefaultIsPagerBlockedBy(View? v) =>
    v is HorizontalScrollView or SeekBar;

  private View? _findBlockingViewUnder(ViewGroup? parent, float x, float y) {
    if (parent == null) return null;

    for (int i = parent.ChildCount - 1; i >= 0; i--) {
      var child = parent.GetChildAt(i);
      if (child == null || child.Visibility != ViewStates.Visible) continue;

      var cx = x + parent.ScrollX - child.Left;
      var cy = y + parent.ScrollY - child.Top;

      if (cx < 0 || cy < 0 || cx >= child.Width || cy >= child.Height) continue;

      if (IsHorizontalGestureConsumer(child)) return child;

      if (IsGestureBoundary(child)) return null;

      if (_findBlockingViewUnder(child as ViewGroup, cx, cy) is View innerChild) return innerChild;

      return null;
    }

    return null;
  }

  public override bool DispatchTouchEvent(MotionEvent? e) {
    if (e == null) return base.DispatchTouchEvent(e);

    switch (e.ActionMasked) {
      case MotionEventActions.Down:
        _isDragging = false;
        _downX = e.GetX();
        _downY = e.GetY();

        var canDrag = AllowDragOnlyFromEdge
          ? _downX <= EdgeSwipeSizePx || _downX >= Width - EdgeSwipeSizePx
          : true;

        _canIntercept = canDrag && !IsPagerBlockedBy(_findBlockingViewUnder(this, _downX, _downY));

        break;
      case MotionEventActions.Move:
        if (_canIntercept && !_isDragging) {
          var dx = e.GetX() - _downX;
          var dy = e.GetY() - _downY;
          var horizontalEnough = Math.Abs(dx) > _touchSlop && Math.Abs(dx) > Math.Abs(dy);
          var directionMatchesEdge = (_downX <= EdgeSwipeSizePx && dx > 0) || (_downX >= Width - EdgeSwipeSizePx && dx < 0);

          if (horizontalEnough && directionMatchesEdge) {
            _sendCancelToChildren(e);
            _startDragFrom(e);
          }
        }
        break;
    }
    return base.DispatchTouchEvent(e);
  }

  private void _sendCancelToChildren(MotionEvent src) {
    if (MotionEvent.Obtain(
      src.DownTime,
      src.EventTime,
      MotionEventActions.Cancel,
      src.GetX(),
      src.GetY(),
      src.MetaState
    ) is not { } cancel) return;

    base.DispatchTouchEvent(cancel);
    cancel.Recycle();
  }

  private void _startDragFrom(MotionEvent e) {
    if (MotionEvent.Obtain(e) is not { } fakeDown) return;
    _isDragging = true;
    fakeDown.Action = MotionEventActions.Down;
    OnTouchEvent(fakeDown);
    fakeDown.Recycle();
  }

  public override bool OnInterceptTouchEvent(MotionEvent? e) =>
    UserInputEnabled && _isDragging;

  public override bool OnTouchEvent(MotionEvent? e) {
    if (e == null || !UserInputEnabled) return false;

    switch (e.ActionMasked) {
      case MotionEventActions.Down:
        _downX = e.GetX();
        _scrollStartX = ScrollX;
        return true;

      case MotionEventActions.Move:
        if (!_isDragging) return false;

        var dx = _downX - e.GetX();
        ScrollTo(_scrollStartX + (int)dx, 0);
        return true;

      case MotionEventActions.Up:
      case MotionEventActions.Cancel:
        if (_isDragging) {
          var pageOffset = (float)ScrollX / MeasuredWidth;

          if (pageOffset > _visibleIndex + 0.3f) {
            _visibleIndex++;
            _reorderChildren();
          } else if (pageOffset < _visibleIndex - 0.3f) {
            _visibleIndex--;
            _reorderChildren();
          }

          _scrollToVisibleIndex();
        }

        _isDragging = false;
        return true;
    }

    return false;
  }

  public override void ComputeScroll() {
    if (_scroller.ComputeScrollOffset()) {
      ScrollTo(_scroller.CurrX, _scroller.CurrY);
      PostInvalidateOnAnimation();
    }
  }
}