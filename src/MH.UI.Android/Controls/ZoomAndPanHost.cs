﻿using Android.Content;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils.Types;
using System;

namespace MH.UI.Android.Controls;

public class ZoomAndPanHost : FrameLayout, IZoomAndPanHost, IDisposable {
  private bool _disposed;
  private ImageView _imageView = null!;
  private readonly global::Android.Graphics.Matrix _matrix = new();
  private ScaleGestureDetector _scaleDetector = null!;
  private GestureDetector _gestureDetector = null!;
  private bool _isPanning;
  private bool _isScaling;
  private ZoomAndPan? _dataContext;

  public ZoomAndPan DataContext { get => _dataContext ?? throw new InvalidOperationException(ErrorMessages.DataContextNotInitialized); }
  double IZoomAndPanHost.Width => Width;
  double IZoomAndPanHost.Height => Height;

  public event EventHandler? HostSizeChangedEvent;
  public event EventHandler<PointD>? HostMouseMoveEvent;
  public event EventHandler<(PointD, PointD)>? HostMouseDownEvent;
  public event EventHandler? HostMouseUpEvent;
  public event EventHandler<(int, PointD)>? HostMouseWheelEvent;
  public event EventHandler? SingleTapConfirmedEvent;

  public ZoomAndPanHost(Context context) : base(context) => _initialize(context);
  public ZoomAndPanHost(Context context, IAttributeSet attrs) : base(context, attrs) => _initialize(context);
  protected ZoomAndPanHost(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) => _initialize(Context!);

  private void _initialize(Context context) {
    _imageView = new ImageView(context) {
      LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
    };
    _imageView.SetScaleType(ImageView.ScaleType.Matrix);
    _scaleDetector = new ScaleGestureDetector(context, new ScaleListener(this));
    _gestureDetector = new GestureDetector(context, new GestureListener(this));
    AddView(_imageView);
  }

  public ZoomAndPanHost Bind(ZoomAndPan? dataContext) {
    _dataContext = dataContext;
    if (dataContext == null) return this;
    dataContext.Host = this;
    return this;
  }

  public void SetImageBitmap(global::Android.Graphics.Bitmap? bitmap) {
    var oldBitmap = _imageView.Drawable is BitmapDrawable bd ? bd.Bitmap : null;
    _imageView.SetImageBitmap(bitmap);
    if (oldBitmap?.IsRecycled == false && bitmap != oldBitmap) oldBitmap.Recycle();
  }

  public void UpdateImageTransform() {
    _matrix.SetScale((float)DataContext.ScaleX, (float)DataContext.ScaleY);
    _matrix.PostTranslate((float)DataContext.TransformX, (float)DataContext.TransformY);
    _imageView.ImageMatrix = _matrix;
  }

  public override bool OnTouchEvent(MotionEvent? e) {
    if (e == null) return base.OnTouchEvent(e);
    _scaleDetector.OnTouchEvent(e);
    _gestureDetector.OnTouchEvent(e);

    switch (e.Action) {
      case MotionEventActions.Down:
        _isPanning = true;
        // TODO contentPos, but it is not used anyway in android
        HostMouseDownEvent?.Invoke(this, new(new(e.GetX(), e.GetY()), new()));
        return true;

      case MotionEventActions.Move:
        if (_scaleDetector.IsInProgress) _isScaling = true;
        if (_isPanning && !_isScaling && DataContext.IsZoomed) {
          HostMouseMoveEvent?.Invoke(this, new PointD(e.GetX(), e.GetY()));
          UpdateImageTransform();
          return true;
        }
        break;

      case MotionEventActions.Up:
        _isPanning = false;
        _isScaling = false;
        return true;
    }
    return base.OnTouchEvent(e);
  }

  protected override void OnSizeChanged(int w, int h, int oldw, int oldh) {
    base.OnSizeChanged(w, h, oldw, oldh);
    HostSizeChangedEvent?.Invoke(this, EventArgs.Empty);
    UpdateImageTransform();
  }

  // TODO make animation in ZoomAndPan optional
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
      HostSizeChangedEvent = null;
      HostMouseMoveEvent = null;
      HostMouseDownEvent = null;
      HostMouseUpEvent = null;
      HostMouseWheelEvent = null;
      SingleTapConfirmedEvent = null;

      DataContext.Host = null;
      _imageView.SetImageBitmap(null);
      _imageView.Dispose();
      _matrix.Dispose();
      _scaleDetector.Dispose();
      _gestureDetector.Dispose();
    }

    _disposed = true;
    base.Dispose(disposing);
  }

  private class ScaleListener(ZoomAndPanHost _host) : ScaleGestureDetector.SimpleOnScaleGestureListener {
    public override bool OnScale(ScaleGestureDetector d) {
      _host.DataContext.Zoom(_host.DataContext.ScaleX + d.ScaleFactor - 1, new PointD(d.FocusX, d.FocusY));
      _host.UpdateImageTransform();
      return true;
    }
  }

  private class GestureListener(ZoomAndPanHost _host) : GestureDetector.SimpleOnGestureListener {
    public override bool OnSingleTapConfirmed(MotionEvent e) {
      _host.SingleTapConfirmedEvent?.Invoke(_host, EventArgs.Empty);
      return true;
    }

    public override bool OnDoubleTap(MotionEvent e) {
      if (_host.DataContext.IsZoomed)
        _host.DataContext.ScaleToFit();
      else
        _host.DataContext.Zoom(1, new PointD(e.GetX(), e.GetY()));

      _host.UpdateImageTransform();
      return true;
    }
  }
}