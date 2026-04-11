using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Transforms;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.UI.Primitives;
using MH.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Android.Widget.ImageView;

namespace MH.UI.Android.Controls;

public class ZoomableVideoView : FrameLayout {
  private readonly ZoomAndPan _zoomAndPan;
  private readonly ImageView _videoPreview;
  private readonly MediaPlayer _mediaPlayer;
  private readonly AndroidMediaPlayer _androidMediaPlayer;
  private readonly IconButton _playBtn;
  private readonly ZoomableVideoSurface _videoView;
  private double _thumbW;
  private double _thumbH;
  private bool _previewOnly;

  public bool PreviewOnly {
    get => _previewOnly;
    set {
      _previewOnly = value;

      if (_previewOnly) {
        _videoView.Visibility = ViewStates.Gone;
        _playBtn.Visibility = ViewStates.Visible;
        _videoPreview.Visibility = ViewStates.Visible;
      } else {
        _videoView.Visibility = ViewStates.Visible;
        _playBtn.Visibility = ViewStates.Gone;
        _videoPreview.Visibility = ViewStates.Gone;
      }
    }
  }

  public ZoomableVideoView(Context context, ZoomAndPan zoomAndPan, MediaPlayer mediaPlayer, AndroidMediaPlayer androidMediaPlayer) : base(context) {
    _zoomAndPan = zoomAndPan;
    _mediaPlayer = mediaPlayer;
    _androidMediaPlayer = androidMediaPlayer;
    _videoView = new(context);

    _androidMediaPlayer.VideoSizeChanged += (_, e) => {
      if (_zoomAndPan.ContentWidth == 0)
        _zoomAndPan.SetContentSize(e.Width, e.Height);
    };

    _videoPreview = new(context);
    _videoPreview.SetScaleType(ScaleType.Matrix);

    _playBtn = new IconButton(context).WithClickAction(_ => _onPlay());
    _playBtn.SetImageResource(Resource.Drawable.icon_question); // TODO icon

    AddView(_videoView, LPU.FrameMatch());
    AddView(_videoPreview, LPU.FrameMatch());
    AddView(_playBtn, LPU.Frame(LPU.Wrap, LPU.Wrap, GravityFlags.Center));

    _zoomAndPan.ViewportChangedEvent += _onViewportChanged;
  }

  public async Task SetPath(string videoPath, MH.Utils.Imaging.Orientation orientation, CancellationToken token, Context context) {
    PreviewOnly = true;

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
        _applyPreviewTransform(_zoomAndPan.GetViewportState());
        _videoPreview.UpdateImageBitmap(thumb);
      });
    }
    catch (Exception ex) {
      MH.Utils.Log.Error(ex);
    }
  }

  private void _applyPreviewTransform(ViewportState state) =>
    _videoPreview.ImageMatrix = ViewportMatrixBuilder.BuildForBitmap(state, _thumbW, _thumbH);

  public void UnsetImage() =>
    _videoPreview.UpdateImageBitmap(null);

  private void _onViewportChanged(object? sender, EventArgs e) {
    var state = _zoomAndPan.GetViewportState();

    if (PreviewOnly)
      _applyPreviewTransform(state);
    else
      _videoView.ApplyTransform(state);
  }

  private void _onPlay() {
    PreviewOnly = false;
    _videoView.Post(() => {
      _videoView.ApplyTransform(_zoomAndPan.GetViewportState());
      _videoView.StartPlayback(_mediaPlayer, _androidMediaPlayer);
    });
  }

  protected override void Dispose(bool disposing) {
    if (disposing) {
      _zoomAndPan.ViewportChangedEvent -= _onViewportChanged;
      UnsetImage();
    }
    base.Dispose(disposing);
  }
}