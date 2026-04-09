using Android.Content;
using Android.Graphics;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Transforms;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MH.UI.Android.Controls;

public class ZoomableImageView : ImageView {
  private readonly ZoomAndPan _zoomAndPan;

  public ZoomableImageView(Context ctx, ZoomAndPan zoomAndPan) : base(ctx) {
    SetScaleType(ScaleType.Matrix);
    _zoomAndPan = zoomAndPan;
    _zoomAndPan.ViewportChangedEvent += _onViewportChanged;
  }

  public async Task SetPath(string path, MH.Utils.Imaging.Orientation orientation, CancellationToken token, Context context) {
    try {
      var thumb = await MediaStoreU.GetImageThumbnail(path, context, 512);
      if (token.IsCancellationRequested) return;
      thumb ??= await Task.Run(() => ImagingU.CreateImageThumbnail(path, 512), token);
      if (thumb is not { Width: > 0, Height: > 0 } || token.IsCancellationRequested) return;
      thumb = thumb.ApplyOrientation(orientation);

      Post(() => {
        if (token.IsCancellationRequested) return;
        ImageMatrix = ViewportMatrixBuilder.BuildForBitmap(_zoomAndPan.GetViewportState(), thumb.Width, thumb.Height);
        this.UpdateImageBitmap(thumb);
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

      Post(() => {
        if (token.IsCancellationRequested) return;
        _applyTransform();
        this.UpdateImageBitmap(bitmap);
      });
    }
    catch (OperationCanceledException) { }
  }

  public void UnsetImage() =>
    this.UpdateImageBitmap(null);

  private void _applyTransform() {
    var state = _zoomAndPan.GetViewportState();
    ImageMatrix = ViewportMatrixBuilder.BuildForBitmap(state, state.ContentWidth, state.ContentHeight);
  }

  private void _onViewportChanged(object? sender, EventArgs e) =>
    _applyTransform();

  protected override void Dispose(bool disposing) {
    if (disposing) {
      _zoomAndPan.ViewportChangedEvent -= _onViewportChanged;
      UnsetImage();
    }
    base.Dispose(disposing);
  }
}