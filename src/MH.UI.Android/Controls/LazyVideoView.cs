using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using System;

namespace MH.UI.Android.Controls;

public class LazyVideoView : FrameLayout {
  private readonly ImageView _preview;
  private readonly IconButton _playBtn;

  private bool _previewOnly = true;
  private bool _canPlay = true;

  public bool PreviewOnly {
    get => _previewOnly;
    private set {
      if (_previewOnly == value) return;

      _previewOnly = value;
      _updateVisibility();
      PreviewOnlyChanged?.Invoke(value);
    }
  }

  public bool CanPlay {
    get => _canPlay;
    set {
      if (_canPlay == value) return;

      _canPlay = value;
      _updateVisibility();
    }
  }

  public VideoSurface VideoSurface { get; private set; }

  public event Action? PlayRequested;
  public event Action<bool>? PreviewOnlyChanged;

  public LazyVideoView(Context context) : base(context) {
    VideoSurface = new VideoSurface(context);

    _preview = new ImageView(context);
    _preview.SetScaleType(ImageView.ScaleType.Matrix);

    _playBtn = new IconButton(context).WithClickAction(_ => RequestPlay());
    _playBtn.SetImageResource(Resource.Drawable.icon_play_circle);

    AddView(VideoSurface, LPU.FrameMatch());
    AddView(_preview, LPU.FrameMatch());
    AddView(_playBtn, LPU.Frame(LPU.Wrap, LPU.Wrap, GravityFlags.Center));

    _updateVisibility();
  }

  public void SetPreview(Bitmap? bitmap) =>
    _preview.UpdateImageBitmap(bitmap);

  public void SetPreviewMatrix(Matrix? matrix) {
    if (matrix != null)
      _preview.ImageMatrix = matrix;
  }

  public void SetVideoMatrix(Matrix matrix) =>
    VideoSurface.SetTransform(matrix);

  public void RequestPlay() {
    ShowVideo();
    Post(() => PlayRequested?.Invoke());
  }

  public void ShowPreview() =>
    PreviewOnly = true;

  public void ShowVideo() =>
    PreviewOnly = false;

  public void Clear() {
    ShowPreview();
    SetPreview(null);
  }

  private void _updateVisibility() {
    if (_previewOnly) {
      _preview.Visibility = ViewStates.Visible;
      _playBtn.Visibility = _canPlay ? ViewStates.Visible : ViewStates.Gone;
      VideoSurface.Visibility = ViewStates.Gone;
    }
    else {
      _preview.Visibility = ViewStates.Gone;
      _playBtn.Visibility = ViewStates.Gone;
      VideoSurface.Visibility = ViewStates.Visible;
    }
  }

  protected override void Dispose(bool disposing) {
    if (disposing)
      _preview.UpdateImageBitmap(null);

    base.Dispose(disposing);
  }
}