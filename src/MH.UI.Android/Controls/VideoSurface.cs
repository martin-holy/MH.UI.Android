using Android.Content;
using Android.Graphics;
using Android.Views;
using System;

namespace MH.UI.Android.Controls;

public class VideoSurface : TextureView {
  public Surface? Surface { get; private set; }

  public event Action<Surface?>? SurfaceChanged;

  public VideoSurface(Context context) : base(context) {
    SurfaceTextureListener = new SurfaceListener(this);
  }

  protected override void Dispose(bool disposing) {
    if (disposing) {
      Surface?.Release();
      Surface = null;
    }
    base.Dispose(disposing);
  }

  private class SurfaceListener : Java.Lang.Object, ISurfaceTextureListener {
    private readonly VideoSurface _view;

    public SurfaceListener(VideoSurface view) {
      _view = view;
    }

    public void OnSurfaceTextureAvailable(SurfaceTexture surface, int w, int h) {
      _view.Surface = new Surface(surface);
      _view.SurfaceChanged?.Invoke(_view.Surface);
    }

    public bool OnSurfaceTextureDestroyed(SurfaceTexture surface) {
      _view.SurfaceChanged?.Invoke(null);
      _view.Surface?.Release();
      _view.Surface = null;
      return true;
    }

    public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int w, int h) { }
    public void OnSurfaceTextureUpdated(SurfaceTexture surface) { }
  }
}