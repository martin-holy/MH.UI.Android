using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Transforms;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MH.UI.Android.Controls;

public class ZoomableVideoView : FrameLayout {
  private readonly ZoomAndPan _zoomAndPan;
  private readonly AndroidMediaPlayer _androidMediaPlayer;

  private readonly LazyVideoView _video;

  private double _thumbW;
  private double _thumbH;
  private int? _lockedHeight;

  public bool PreviewOnly => _video.PreviewOnly;
  public bool AutoHeightFromAspectRatio { get; set; }

  public event Action? PlayRequested;
  public event Action<bool>? PreviewOnlyChanged;

  public ZoomableVideoView(Context context, ZoomAndPan zoomAndPan, AndroidMediaPlayer androidMediaPlayer) : base(context) {
    _zoomAndPan = zoomAndPan;
    _zoomAndPan.ViewportChangedEvent += _onViewportChanged;

    _androidMediaPlayer = androidMediaPlayer;
    _androidMediaPlayer.VideoSizeChanged += _onVideoSizeChanged;

    _video = new LazyVideoView(context);
    _video.PlayRequested += _onPlayRequested;
    _video.PreviewOnlyChanged += _onPreviewOnlyChanged;
    _video.VideoSurface.SurfaceChanged += _onSurfaceChanged;

    AddView(_video, LPU.FrameMatch());
  }

  public void ShowPreview() => _video.ShowPreview();

  public void RequestPlay() => _video.RequestPlay();

  public void Clear() => _video.Clear();

  public async Task SetPath(string videoPath, MH.Utils.Imaging.Orientation orientation, CancellationToken token, Context context) {
    _video.ShowPreview();
    _lockedHeight = null;

    try {
      var thumb = await MediaStoreU.GetVideoThumbnail(videoPath, context, 512);
      if (token.IsCancellationRequested) return;
      thumb ??= await Task.Run(() => ImagingU.CreateVideoThumbnail(videoPath, 512), token);
      if (thumb is not { Width: > 0, Height: > 0 } || token.IsCancellationRequested) return;
      thumb = thumb.ApplyOrientation(orientation);

      Post(() => {
        if (token.IsCancellationRequested) return;
        _thumbW = thumb.Width;
        _thumbH = thumb.Height;
        _video.SetPreviewMatrix(ViewportMatrixBuilder.BuildForBitmap(_zoomAndPan.GetViewportState(), _thumbW, _thumbH));
        _video.SetPreview(thumb);
      });
    }
    catch (Exception ex) {
      MH.Utils.Log.Error(ex);
    }
  }

  private void _onVideoSizeChanged(object? sender, (int Width, int Height) e) {
    if (e.Width <= 0 || e.Height <= 0) return;

    if (_lockedHeight.HasValue) {
      var expected = (int)Math.Round((double)Width * e.Height / e.Width);

      if (expected != _lockedHeight.Value)
        _lockedHeight = expected;
    }

    _zoomAndPan.SetContentSize(e.Width, e.Height);
  }

  protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {
    if (!AutoHeightFromAspectRatio) {
      base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
      return;
    }

    int width = MeasureSpec.GetSize(widthMeasureSpec);

    if (width <= 0) {
      base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
      return;
    }

    int height;

    if (_lockedHeight.HasValue) {
      height = _lockedHeight.Value;
    }
    else if (_zoomAndPan.ContentWidth > 0 && _zoomAndPan.ContentHeight > 0) {
      double aspect = _zoomAndPan.ContentHeight / _zoomAndPan.ContentWidth;
      height = (int)Math.Round(width * aspect);

      _lockedHeight = height;
    }
    else {
      height = MeasureSpec.GetSize(heightMeasureSpec);
    }

    SetMeasuredDimension(width, height);
    MeasureChildren(
      MeasureSpec.MakeMeasureSpec(width, MeasureSpecMode.Exactly),
      MeasureSpec.MakeMeasureSpec(height, MeasureSpecMode.Exactly));
  }

  private void _onViewportChanged(object? sender, EventArgs e) {
    var state = _zoomAndPan.GetViewportState();

    if (_video.PreviewOnly)
      _video.SetPreviewMatrix(ViewportMatrixBuilder.BuildForBitmap(state, _thumbW, _thumbH));
    else
      _video.SetVideoMatrix(ViewportMatrixBuilder.BuildForTextureView(state));
  }

  private void _onPlayRequested() {
    _lockedHeight = Height;
    _video.SetVideoMatrix(ViewportMatrixBuilder.BuildForTextureView(_zoomAndPan.GetViewportState()));
    PlayRequested?.Invoke();
  }

  private void _onPreviewOnlyChanged(bool value) =>
    PreviewOnlyChanged?.Invoke(value);

  private void _onSurfaceChanged(Surface? surface) =>
    _androidMediaPlayer.SetSurface(surface);

  protected override void Dispose(bool disposing) {
    if (disposing) {
      _zoomAndPan.ViewportChangedEvent -= _onViewportChanged;
      _androidMediaPlayer.VideoSizeChanged -= _onVideoSizeChanged;
      _video.PlayRequested -= _onPlayRequested;
      _video.PreviewOnlyChanged -= _onPreviewOnlyChanged;
      _video.VideoSurface.SurfaceChanged -= _onSurfaceChanged;
      _video.Clear();
    }
    base.Dispose(disposing);
  }
}