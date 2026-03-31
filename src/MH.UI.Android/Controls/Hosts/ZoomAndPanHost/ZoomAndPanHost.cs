using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MH.UI.Android.Controls.Hosts.ZoomAndPanHost;

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

  public event EventHandler? SingleTapConfirmedEvent;
  public event EventHandler? ImageTransformUpdatedEvent;

  public ZoomAndPanHost(Context context, ZoomAndPan dataContext) : base(context) {
    DataContext = dataContext;
    dataContext.Host = this;

    _imageView = new ImageView(context);
    _imageView.SetScaleType(ImageView.ScaleType.Matrix);

    _videoView = new VideoView(context) { Visibility = ViewStates.Gone };
    _videoView.SetZOrderOnTop(false);

    _scaleDetector = new ScaleGestureDetector(context, new ScaleListener(this));
    _gestureDetector = new GestureDetector(context, new GestureListener(this));

    AddView(_imageView, LPU.FrameMatch());
    AddView(_videoView, LPU.FrameMatch());
  }

  [Obsolete("Use overload without width and height")]
  public async Task SetImagePathAsync(string path, double width, double height, MH.Utils.Imaging.Orientation orientation, CancellationToken token, Context context) =>
    await SetImagePathAsync(path, orientation, token, context);

  public async Task SetImagePathAsync(string path, MH.Utils.Imaging.Orientation orientation, CancellationToken token, Context context) {
    _showingVideo = false;
    _videoView.Visibility = ViewStates.Gone;
    _imageView.Visibility = ViewStates.Visible;

    try {
      var thumb = await MediaStoreU.GetImageThumbnail(path, context, 512);
      if (token.IsCancellationRequested) return;
      thumb ??= await Task.Run(() => ImagingU.CreateImageThumbnail(path, 512), token);
      if (thumb is not { Width: > 0, Height: > 0 } || token.IsCancellationRequested) return;
      thumb = thumb.ApplyOrientation(orientation);

      _imageView.Post(() => {
        if (token.IsCancellationRequested) return;
        _applyThumbnailMatrix(thumb);
        _setImageBitmap(thumb);
      });

      if (!token.IsCancellationRequested)
        _ = _loadFullImageAsync(path, orientation, token);
    }
    catch (Exception ex) {
      MH.Utils.Log.Error(ex);
    }
  }

  private async Task _loadFullImageAsync(string path, MH.Utils.Imaging.Orientation orientation, CancellationToken token) {
    try {
      var bitmap = await Task.Run(() => {
        token.ThrowIfCancellationRequested();
        var bmp = BitmapFactory.DecodeFile(path);
        token.ThrowIfCancellationRequested();
        return bmp?.ApplyOrientation(orientation);
      }, token);

      if (bitmap == null || token.IsCancellationRequested) return;

      _imageView.Post(() => {
        if (token.IsCancellationRequested) return;
        _setImageBitmap(bitmap);
        UpdateImageTransform();
      });
    }
    catch (OperationCanceledException) { }
  }

  public void UnsetImage() =>
    _setImageBitmap(null);

  private void _setImageBitmap(Bitmap? bitmap) {
    var oldBitmap = _imageView.Drawable is BitmapDrawable bd ? bd.Bitmap : null;
    _imageView.SetImageBitmap(bitmap);
    if (oldBitmap?.IsRecycled == false && bitmap != oldBitmap) oldBitmap.Recycle();
  }

  private void _applyThumbnailMatrix(Bitmap thumb) {
    var scale = DataContext.GetFitScale();
    var ratio = scale * (DataContext.ContentWidth / thumb.Width);

    var tx = (Width - (thumb.Width * ratio)) / 2;
    var ty = (Height - (thumb.Height * ratio)) / 2;

    _matrix.Reset();
    _matrix.SetScale((float)ratio, (float)ratio);
    _matrix.PostTranslate((float)tx, (float)ty);

    _imageView.ImageMatrix = _matrix;
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

    _matrix.Reset();
    _matrix.SetScale((float)DataContext.ScaleX, (float)DataContext.ScaleY);
    _matrix.PostTranslate((float)DataContext.TransformX, (float)DataContext.TransformY);
    _imageView.ImageMatrix = _matrix;

    ImageTransformUpdatedEvent?.Invoke(this, EventArgs.Empty);
  }

  public override bool OnTouchEvent(MotionEvent? e) {
    if (e == null) return base.OnTouchEvent(e);

    if (_showingVideo) return base.OnTouchEvent(e); // disable zoom/pan on video for now

    _scaleDetector.OnTouchEvent(e);
    _gestureDetector.OnTouchEvent(e);

    switch (e.Action) {
      case MotionEventActions.Down:
        _isPanning = true;
        DataContext.PointerDown(new(e.GetX(), e.GetY()));
        return true;

      case MotionEventActions.Move:
        if (_scaleDetector.IsInProgress) _isScaling = true;
        if (_isPanning && !_isScaling && DataContext.IsZoomed) {
          DataContext.PointerMove(new(e.GetX(), e.GetY()));
          UpdateImageTransform();
          return true;
        }
        break;

      case MotionEventActions.Up:
        _isPanning = false;
        _isScaling = false;
        return true;
    }
    return DataContext.IsZoomed || _isScaling;
  }

  protected override void OnSizeChanged(int w, int h, int oldw, int oldh) {
    base.OnSizeChanged(w, h, oldw, oldh);
    DataContext.SetHostSize(w, h);
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
      _host.DataContext.Zoom(d.ScaleFactor, new PointD(d.FocusX, d.FocusY));
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

      _host.DataContext.ToggleZoom(new PointD(e.GetX(), e.GetY()));
      _host.UpdateImageTransform();
      return true;
    }
  }
}