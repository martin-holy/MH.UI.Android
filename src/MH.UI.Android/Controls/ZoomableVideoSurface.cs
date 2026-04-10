using Android.Content;
using Android.Graphics;
using Android.Views;
using MH.UI.Android.Transforms;
using MH.UI.Primitives;

namespace MH.UI.Android.Controls;

public class ZoomableVideoSurface : TextureView {
  private Surface? _surface;
  private AndroidMediaPlayer? _player;

  public ZoomableVideoSurface(Context ctx) : base(ctx) {
    SurfaceTextureListener = new SurfaceListener(this);
  }

  public void ApplyTransform(ViewportState state) =>
    SetTransform(ViewportMatrixBuilder.BuildForTextureView(state));

  public void StartPlayback(MH.UI.Controls.MediaPlayer mediaPlayer, AndroidMediaPlayer androidMediaPlayer) {
    _player = androidMediaPlayer;
    mediaPlayer.SetView(_player);
    _player.SetSurface(_surface);
    mediaPlayer.IsPlaying = true;
  }

  protected override void Dispose(bool disposing) {
    if (disposing) {
      _surface?.Release();
      _surface = null;
    }
    base.Dispose(disposing);
  }

  private class SurfaceListener : Java.Lang.Object, ISurfaceTextureListener {
    private readonly ZoomableVideoSurface _view;

    public SurfaceListener(ZoomableVideoSurface view) {
      _view = view;
    }

    public void OnSurfaceTextureAvailable(SurfaceTexture surface, int w, int h) {
      _view._surface = new Surface(surface);

      if (_view._player != null)
        _view._player.SetSurface(_view._surface);
    }

    public bool OnSurfaceTextureDestroyed(SurfaceTexture surface) {
      _view._player?.SetSurface(null);

      _view._surface?.Release();
      _view._surface = null;
      return true;
    }

    public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int w, int h) { }
    public void OnSurfaceTextureUpdated(SurfaceTexture surface) { }
  }
}