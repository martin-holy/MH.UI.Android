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
  private readonly global::Android.Graphics.Matrix _matrix = new();
  private ScaleGestureDetector _scaleDetector = null!;
  private GestureDetector _gestureDetector = null!;
  private bool _isPanning;
  private ZoomAndPan? _dataContext;

  public ZoomAndPan DataContext { get => _dataContext ?? throw new NotImplementedException(); }
  double IZoomAndPanHost.Width => Width;
  double IZoomAndPanHost.Height => Height;

  public event EventHandler? HostSizeChangedEvent;
  public event EventHandler<PointD>? HostMouseMoveEvent;
  public event EventHandler<(PointD, PointD)>? HostMouseDownEvent;
  public event EventHandler? HostMouseUpEvent;
  public event EventHandler<(int, PointD)>? HostMouseWheelEvent;
  public event EventHandler? SingleTapConfirmedEvent;

  public ZoomAndPanHost(Context context) : base(context) => _initialize(context);

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
    _setDataContext(dataContext);
    if (dataContext == null) return this;
    dataContext.Host = this;
    return this;
  }

  private void _setDataContext(ZoomAndPan? value) {
    if (_dataContext != null)
      _dataContext.PropertyChanged -= _onDataContextPropertyChanged;
    _dataContext = value;
    if (_dataContext != null)
      _dataContext.PropertyChanged += _onDataContextPropertyChanged;
  }

  public void SetImageBitmap(global::Android.Graphics.Bitmap? bitmap) =>
    _imageView.SetImageBitmap(bitmap);

  private void _onDataContextPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    // TODO agregate change if needed. try not to do it this way
    /*if (e.PropertyName is
        nameof(ZoomAndPan.ScaleX) or nameof(ZoomAndPan.ScaleY) or
        nameof(ZoomAndPan.TransformX) or nameof(ZoomAndPan.TransformY) or
        nameof(ZoomAndPan.ContentWidth) or nameof(ZoomAndPan.ContentHeight)) {
      _updateImageTransform();
    }*/
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
        if (_isPanning && !_scaleDetector.IsInProgress) {
          HostMouseMoveEvent?.Invoke(this, new PointD(e.GetX(), e.GetY()));
          UpdateImageTransform();
          return true;
        }
        break;

      case MotionEventActions.Up:
        _isPanning = false;
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