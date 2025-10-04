using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager2.Widget;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils;
using System;

namespace MH.UI.Android.Controls;

public class SlidePanelsGridHost : LinearLayout {
  private readonly ViewPager2 _viewPager;
  private readonly View _panelLeft;
  private readonly View _panelTop;
  private readonly View _panelRight;
  private readonly View _panelBottom;
  private readonly View _panelMiddle;
  private bool _disposed;

  public ViewPager2 ViewPager { get => _viewPager; }
  public SlidePanelsGrid DataContext { get; }

  public SlidePanelsGridHost(Context context, SlidePanelsGrid dataContext, View left, View top, View right, View bottom, View middle) : base(context) {
    DataContext = dataContext;
    _panelLeft = left;
    _panelTop = top;
    _panelRight = right;
    _panelBottom = bottom;
    _panelMiddle = middle;
    _viewPager = new(context) { Adapter = new PanelAdapter(this) };

    Orientation = Orientation.Vertical;
    SetBackgroundResource(Resource.Color.c_static_ba);

    this.Bind(dataContext.PanelTop, x => x.IsPinned, (v, p) => v._panelTop.Visibility = p ? ViewStates.Visible : ViewStates.Gone);
    this.Bind(dataContext.PanelBottom, x => x.IsPinned, (v, p) => v._panelBottom.Visibility = p ? ViewStates.Visible : ViewStates.Gone);

    AddView(top, new LayoutParams(LPU.Match, LPU.Wrap));
    AddView(_viewPager, new LayoutParams(LPU.Match, 0, 1f));
    AddView(bottom, new LayoutParams(LPU.Match, LPU.Wrap));
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      var adapter = _viewPager.Adapter;
      _viewPager.Adapter = null;
      adapter?.Dispose();
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  private class PanelAdapter(SlidePanelsGridHost _host) : RecyclerView.Adapter {
    public override int ItemCount => 3;

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
      var view = viewType switch {
        0 => _host._panelLeft,
        1 => _host._panelMiddle,
        2 => _host._panelRight,
        _ => throw new ArgumentOutOfRangeException(nameof(viewType))
      };
      view.LayoutParameters = new RecyclerView.LayoutParams(LPU.Match, LPU.Match);
      return new PanelViewHolder(view);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) { }

    public override int GetItemViewType(int position) => position;
  }

  private class PanelViewHolder(View itemView) : RecyclerView.ViewHolder(itemView);
}