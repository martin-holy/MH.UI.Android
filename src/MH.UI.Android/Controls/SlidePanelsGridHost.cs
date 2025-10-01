using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager2.Widget;
using MH.UI.Android.Utils;
using System;

namespace MH.UI.Android.Controls;

public class SlidePanelsGridHost : LinearLayout {
  private readonly ViewPager2 _viewPager;
  private readonly LinearLayout _topPanel;
  private readonly LinearLayout _bottomPanel;
  private readonly Func<int, View> _panelFactory;

  public ViewPager2 ViewPager { get => _viewPager; }

  public SlidePanelsGridHost(Context context, Func<int, View> panelFactory) : base(context) {
    _panelFactory = panelFactory;
    Orientation = Orientation.Vertical;
    SetBackgroundResource(Resource.Color.c_static_ba);

    _topPanel = new(context);
    _viewPager = new(context) { Adapter = new PanelAdapter(this) };
    _bottomPanel = new(context) { Visibility = ViewStates.Gone };

    AddView(_topPanel, new LayoutParams(LPU.Match, LPU.Wrap));
    AddView(_viewPager, new LayoutParams(LPU.Match, 0, 1f));
    AddView(_bottomPanel, new LayoutParams(LPU.Match, LPU.Wrap));
  }

  public void SetTopPanel(View view) {
    _topPanel.RemoveAllViews();
    _topPanel.AddView(view);
  }

  public void SetBottomPanel(View view, bool isVisible) {
    _bottomPanel.RemoveAllViews();
    _bottomPanel.AddView(view);
    _bottomPanel.Visibility = isVisible ? ViewStates.Visible : ViewStates.Gone;
  }

  private class PanelAdapter(SlidePanelsGridHost _host) : RecyclerView.Adapter {
    public override int ItemCount => 3; // Left, Middle, Right

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
      var view = _host._panelFactory(viewType);
      view.LayoutParameters = new RecyclerView.LayoutParams(LPU.Match, LPU.Match);
      return new PanelViewHolder(view);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) { }

    public override int GetItemViewType(int position) => position; // 0=Left, 1=Middle, 2=Right
  }

  private class PanelViewHolder(View itemView) : RecyclerView.ViewHolder(itemView);
}