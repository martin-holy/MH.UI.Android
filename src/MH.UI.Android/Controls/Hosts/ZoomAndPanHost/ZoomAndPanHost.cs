using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Controls;
using MH.Utils.Types;
using System;

namespace MH.UI.Android.Controls.Hosts.ZoomAndPanHost;

public class ZoomAndPanHost : FrameLayout, IZoomAndPanHost {
  private readonly ScaleGestureDetector _scaleDetector;
  private readonly GestureDetector _gestureDetector;
  private bool _isPanning;
  private bool _isScaling;
  private bool _disposed;

  public ZoomAndPan DataContext { get; }

  public event EventHandler? SingleTapConfirmedEvent;

  public ZoomAndPanHost(Context context, ZoomAndPan dataContext) : base(context) {
    DataContext = dataContext;
    dataContext.Host = this;

    _scaleDetector = new(context, new ScaleListener(this));
    _gestureDetector = new(context, new GestureListener(this));
  }

  public override bool OnTouchEvent(MotionEvent? e) {
    if (e == null) return base.OnTouchEvent(e);

    _scaleDetector.OnTouchEvent(e);
    _gestureDetector.OnTouchEvent(e);

    switch (e.Action) {
      case MotionEventActions.Down:
        _isPanning = true;
        DataContext.PointerDown(new(e.GetX(), e.GetY()));
        return true;

      case MotionEventActions.Move:
        if (_scaleDetector.IsInProgress) _isScaling = true;
        if (_isPanning && !_isScaling && DataContext.IsOverflowing) {
          DataContext.PointerMove(new(e.GetX(), e.GetY()));
          return true;
        }
        break;

      case MotionEventActions.Up:
        _isPanning = false;
        _isScaling = false;
        return true;
    }
    return DataContext.IsOverflowing || _isScaling;
  }

  protected override void OnSizeChanged(int w, int h, int oldw, int oldh) {
    base.OnSizeChanged(w, h, oldw, oldh);
    DataContext.SetHostSize(w, h);
  }

  public void StartAnimation(double toValue, double duration, bool horizontal, Action onCompleted) {
    // Not implemented (animation skipped)
    onCompleted();
  }

  public void StopAnimation() {
    // Not implemented
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      DataContext.Host = null;
      _scaleDetector.Dispose();
      _gestureDetector.Dispose();
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  private class ScaleListener(ZoomAndPanHost _host) : ScaleGestureDetector.SimpleOnScaleGestureListener {
    public override bool OnScale(ScaleGestureDetector d) {
      _host.DataContext.Zoom(d.ScaleFactor, new PointD(d.FocusX, d.FocusY));
      return true;
    }
  }

  private class GestureListener(ZoomAndPanHost _host) : GestureDetector.SimpleOnGestureListener {
    public override bool OnSingleTapConfirmed(MotionEvent e) {
      _host.SingleTapConfirmedEvent?.Invoke(_host, EventArgs.Empty);
      return true;
    }

    public override bool OnDoubleTap(MotionEvent e) {
      _host.DataContext.ToggleZoom(new PointD(e.GetX(), e.GetY()));
      return true;
    }
  }
}