using Android.Content;
using Android.Graphics;
using Android.Views;

namespace MH.UI.Android.Controls;

public class VideoSurface : TextureView {
  private Surface? _surface;
  private AndroidMediaPlayer? _player;

  public VideoSurface(Context ctx) : base(ctx) {
    SurfaceTextureListener = new SurfaceListener(this);
  }

  public void SetSurface(AndroidMediaPlayer androidMediaPlayer) {
    _player = androidMediaPlayer;
    _player.SetSurface(_surface);
  }

  protected override void Dispose(bool disposing) {
    if (disposing) {
      _surface?.Release();
      _surface = null;
    }
    base.Dispose(disposing);
  }

  private class SurfaceListener : Java.Lang.Object, ISurfaceTextureListener {
    private readonly VideoSurface _view;

    public SurfaceListener(VideoSurface view) {
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