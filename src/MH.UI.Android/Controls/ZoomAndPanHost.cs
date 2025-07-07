using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Controls;
using MH.Utils.Types;
using System;
using System.ComponentModel;

namespace MH.UI.Android.Controls;

public class ZoomAndPanHost : FrameLayout, IZoomAndPanHost {
  private ImageView _imageView = null!;
  private ScaleGestureDetector _scaleDetector = null!;
  private float _lastTouchX;
  private float _lastTouchY;
  private bool _isPanning;
  private ZoomAndPan? _dataContext;

  public ZoomAndPan DataContext { get => _dataContext ?? throw new NotImplementedException(); }
  double IZoomAndPanHost.Width => MeasuredWidth;
  double IZoomAndPanHost.Height => MeasuredHeight;

  public event EventHandler? HostSizeChangedEvent;
  public event EventHandler<PointD>? HostMouseMoveEvent;
  public event EventHandler<(PointD, PointD)>? HostMouseDownEvent;
  public event EventHandler? HostMouseUpEvent;
  public event EventHandler<(int, PointD)>? HostMouseWheelEvent;

  public ZoomAndPanHost(Context context) : base(context) => _initialize(context);

  private void _initialize(Context context) {
    _imageView = new ImageView(context) {
      LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
    };
    _scaleDetector = new ScaleGestureDetector(context, new ScaleListener(this));
    AddView(_imageView);
  }

  public ZoomAndPanHost Bind(ZoomAndPan? dataContext) {
    _setDataContext(dataContext);
    if (dataContext == null) return this;
    dataContext.Host = this;
    _updateImageTransform();
    return this;
  }

  private void _setDataContext(ZoomAndPan? value) {
    if (_dataContext != null)
      _dataContext.PropertyChanged -= _onDataContextPropertyChanged;
    _dataContext = value;
    if (_dataContext != null)
      _dataContext.PropertyChanged += _onDataContextPropertyChanged;
  }

  public void SetImageBitmap(global::Android.Graphics.Bitmap? bitmap) {
    _imageView.SetImageBitmap(bitmap);
    if (bitmap != null)
      DataContext.ScaleToFitContent(bitmap.Width, bitmap.Height);
  }

  private void _onDataContextPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    // TODO agregate change
    if (e.PropertyName is
        nameof(ZoomAndPan.ScaleX) or nameof(ZoomAndPan.ScaleY) or
        nameof(ZoomAndPan.TransformX) or nameof(ZoomAndPan.TransformY) or
        nameof(ZoomAndPan.ContentWidth) or nameof(ZoomAndPan.ContentHeight)) {
      _updateImageTransform();
    }
  }

  private void _updateImageTransform() {
    //return; // TODO for now
    var matrix = new global::Android.Graphics.Matrix();
    matrix.SetScale((float)DataContext.ScaleX, (float)DataContext.ScaleY);
    matrix.PostTranslate((float)DataContext.TransformX, (float)DataContext.TransformY);
    _imageView.ImageMatrix = matrix;
    _imageView.LayoutParameters.Width = (int)DataContext.ContentWidth;
    _imageView.LayoutParameters.Height = (int)DataContext.ContentHeight;
    _imageView.RequestLayout(); // TODO test it without it
  }

  public override bool OnTouchEvent(MotionEvent? e) {
    return base.OnTouchEvent(e); // TODO for now

    if (e == null) return base.OnTouchEvent(e);
    _scaleDetector.OnTouchEvent(e);

    switch (e.Action) {
      case MotionEventActions.Down:
        _lastTouchX = e.GetX();
        _lastTouchY = e.GetY();
        _isPanning = true;
        var contentPos = new PointD(
          (_lastTouchX - DataContext.TransformX) / DataContext.ScaleX,
          (_lastTouchY - DataContext.TransformY) / DataContext.ScaleY);
        HostMouseDownEvent?.Invoke(this, (new PointD(_lastTouchX, _lastTouchY), contentPos));
        return true;

      case MotionEventActions.Move:
        if (_isPanning && !_scaleDetector.IsInProgress) {
          float x = e.GetX();
          float y = e.GetY();
          HostMouseMoveEvent?.Invoke(this, new PointD(x, y));
          return true;
        }
        break;

      case MotionEventActions.Up:
        _isPanning = false;
        HostMouseUpEvent?.Invoke(this, EventArgs.Empty);
        return true;
    }
    return base.OnTouchEvent(e);
  }

  protected override void OnSizeChanged(int w, int h, int oldw, int oldh) {
    base.OnSizeChanged(w, h, oldw, oldh);
    HostSizeChangedEvent?.Invoke(this, EventArgs.Empty);
  }

  public void StartAnimation(double toValue, double duration, bool horizontal, Action onCompleted) {
    // Not implemented (animation skipped)
    onCompleted();
  }

  public void StopAnimation() {
    // Not implemented
  }

  private class ScaleListener : ScaleGestureDetector.SimpleOnScaleGestureListener {
    private readonly ZoomAndPanHost _host;

    public ScaleListener(ZoomAndPanHost host) => _host = host;

    public override bool OnScale(ScaleGestureDetector detector) {
      var scaleFactor = detector.ScaleFactor;
      var focusX = detector.FocusX;
      var focusY = detector.FocusY;
      var contentPos = new PointD(
        (focusX - _host.DataContext.TransformX) / _host.DataContext.ScaleX,
        (focusY - _host.DataContext.TransformY) / _host.DataContext.ScaleY);
      _host.DataContext.Zoom(scaleFactor, contentPos);
      return true;
    }
  }
}