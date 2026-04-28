using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils;
using MH.Utils.Disposables;
using System;
using Dock = MH.UI.Controls.Dock;

namespace MH.UI.Android.Controls.Hosts.SlidePanelsGridHost;

public class SlidePanelHost : FrameLayout {
  private readonly SlidePanel _dataContext;

  public SlidePanelHost(Context context, View content, SlidePanel dataContext, BindingScope bindings) : base(context) {
    _dataContext = dataContext;
    _dataContext.Bind(nameof(SlidePanel.IsOpen), x => x.IsOpen, _ => _refreshPosition(true), false).DisposeWith(bindings);
    this.WithClickAction(_onClick);
    AddView(content, LPU.FrameMatch());
  }

  private static void _onClick(SlidePanelHost self) =>
    self._dataContext.TogglePinned();

  protected override void OnSizeChanged(int w, int h, int oldw, int oldh) {
    base.OnSizeChanged(w, h, oldw, oldh);
    _dataContext.Size = _dataContext.Dock is Dock.Top or Dock.Bottom ? h : w;
    _refreshPosition(false);
  }

  private void _refreshPosition(bool animated) {
    float targetX = 0f;
    float targetY = 0f;

    if (!_dataContext.IsOpen) {
      switch (_dataContext.Dock) {
        case Dock.Left: targetX = -Width; break;
        case Dock.Right: targetX = Width; break;
        case Dock.Top: targetY = -Height; break;
        case Dock.Bottom: targetY = Height; break;
      }
    }

    if (animated)
      _animateTo(targetX, targetY);
    else
      _setTransform(targetX, targetY);
  }

  private void _animateTo(float x, float y) {
    Animate()?.Cancel();

    var distance = Math.Max(Math.Abs(TranslationX - x), Math.Abs(TranslationY - y));
    var ms = Math.Max(120, (long)(distance * 0.7));

    Animate()?
      .TranslationX(x)
      .TranslationY(y)
      .SetDuration(ms)
      .Start();
  }

  private void _setTransform(float x, float y) {
    Animate()?.Cancel();
    TranslationX = x;
    TranslationY = y;
  }
}