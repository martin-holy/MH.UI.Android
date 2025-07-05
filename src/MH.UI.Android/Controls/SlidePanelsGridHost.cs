using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager2.Widget;
using System;

namespace MH.UI.Android.Controls;

public class SlidePanelsGridHost : LinearLayout {
  private ViewPager2 _viewPager = null!;
  private LinearLayout _topPanel = null!;
  private LinearLayout _bottomPanel = null!;
  private Func<int, View>? _panelFactory;

  public ViewPager2 ViewPager { get => _viewPager; }

  public SlidePanelsGridHost(Context context) : base(context) => _initialize(context);
  public SlidePanelsGridHost(Context context, IAttributeSet attrs) : base(context, attrs) => _initialize(context);
  protected SlidePanelsGridHost(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) => _initialize(Context!);

  private void _initialize(Context context) {
    Orientation = Orientation.Vertical;
    SetBackgroundResource(Resource.Color.c_static_ba);

    _topPanel = new(context) { LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent) };
    AddView(_topPanel);

    _viewPager = new(context) {
      LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, 0, 1f),
      Adapter = new PanelAdapter(this)
    };
    AddView(_viewPager);

    _bottomPanel = new(context) {
      LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
      Visibility = ViewStates.Gone
    };
    AddView(_bottomPanel);
  }

  public void SetPanelFactory(Func<int, View> panelFactory) {
    _panelFactory = panelFactory ?? throw new ArgumentNullException(nameof(panelFactory));
    _viewPager.Adapter?.NotifyDataSetChanged();
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

  private class PanelAdapter(SlidePanelsGridHost host) : RecyclerView.Adapter {
    private readonly SlidePanelsGridHost _host = host;

    public override int ItemCount => 3; // Left, Middle, Right

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
      var view = _host._panelFactory?.Invoke(viewType) ?? throw new InvalidOperationException("Panel factory not set");
      view.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
      return new PanelViewHolder(view);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) { }

    public override int GetItemViewType(int position) => position; // 0=Left, 1=Middle, 2=Right
  }

  private class PanelViewHolder(View itemView) : RecyclerView.ViewHolder(itemView);
}