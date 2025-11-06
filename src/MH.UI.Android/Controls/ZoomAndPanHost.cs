using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils.Types;
using System;

namespace MH.UI.Android.Controls;

public class ZoomAndPanHost : FrameLayout, IZoomAndPanHost {
  private readonly ImageView _imageView;
  private readonly VideoView _videoView;
  private readonly global::Android.Graphics.Matrix _matrix = new();
  private readonly ScaleGestureDetector _scaleDetector;
  private readonly GestureDetector _gestureDetector;
  private bool _isPanning;
  private bool _isScaling;
  private bool _disposed;
  private bool _showingVideo;

  public ZoomAndPan DataContext { get; }
  double IZoomAndPanHost.Width => Width;
  double IZoomAndPanHost.Height => Height;

  public event EventHandler? HostSizeChangedEvent;
  public event EventHandler<PointD>? HostMouseMoveEvent;
  public event EventHandler<(PointD, PointD)>? HostMouseDownEvent;
  public event EventHandler? HostMouseUpEvent;
  public event EventHandler<(int, PointD)>? HostMouseWheelEvent;
  public event EventHandler? SingleTapConfirmedEvent;

  public ZoomAndPanHost(Context context, ZoomAndPan dataContext) : base(context) {
    DataContext = dataContext;
    dataContext.Host = this;

    _imageView = new ImageView(context);
    _imageView.SetScaleType(ImageView.ScaleType.Matrix);

    _videoView = new VideoView(context) { Visibility = ViewStates.Gone };
    _videoView.SetZOrderOnTop(false);

    _scaleDetector = new ScaleGestureDetector(context, new ScaleListener(this));
    _gestureDetector = new GestureDetector(context, new GestureListener(this));

    AddView(_imageView, new LayoutParams(LPU.Match, LPU.Match));
    AddView(_videoView, new LayoutParams(LPU.Match, LPU.Match));
  }

  public void SetImagePath(string? path) {
    var bitmap = string.IsNullOrEmpty(path)
      ? null
      : global::Android.Graphics.BitmapFactory.DecodeFile(path);

    _showingVideo = false;
    _videoView.Visibility = ViewStates.Gone;
    _imageView.Visibility = ViewStates.Visible;

    var oldBitmap = _imageView.Drawable is BitmapDrawable bd ? bd.Bitmap : null;
    _imageView.SetImageBitmap(bitmap);
    if (oldBitmap?.IsRecycled == false && bitmap != oldBitmap) oldBitmap.Recycle();
  }

  public void SetVideoPath(string? path) {
    if (string.IsNullOrEmpty(path)) {
      if (_showingVideo && _videoView.IsPlaying)
        _videoView.StopPlayback();

      return;
    }

    _showingVideo = true;
    _imageView.Visibility = ViewStates.Gone;
    _videoView.Visibility = ViewStates.Visible;

    _videoView.SetVideoPath(path);
    _videoView.Start();
  }

  public void UpdateImageTransform() {
    if (_showingVideo) return; // Skip zoom transform for now

    _matrix.SetScale((float)DataContext.ScaleX, (float)DataContext.ScaleY);
    _matrix.PostTranslate((float)DataContext.TransformX, (float)DataContext.TransformY);
    _imageView.ImageMatrix = _matrix;
  }

  public override bool OnTouchEvent(MotionEvent? e) {
    if (e == null) return base.OnTouchEvent(e);

    if (_showingVideo) return base.OnTouchEvent(e); // disable zoom/pan on video for now

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
      _imageView.SetImageBitmap(null);
      _videoView.StopPlayback();
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
      if (_host._showingVideo) return false; // Skip zoom toggle on video for now

      if (_host.DataContext.IsZoomed)
        _host.DataContext.ScaleToFit();
      else
        _host.DataContext.Zoom(1, new PointD(e.GetX(), e.GetY()));

      _host.UpdateImageTransform();
      return true;
    }
  }
}