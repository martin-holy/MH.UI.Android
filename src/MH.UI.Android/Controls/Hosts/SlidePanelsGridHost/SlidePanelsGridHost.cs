using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Binding;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils.Disposables;

namespace MH.UI.Android.Controls.Hosts.SlidePanelsGridHost;

public class SlidePanelsGridHost : LinearLayout {
  private readonly LoopPager _viewPager;
  private readonly View _panelLeft;
  private readonly View _panelTop;
  private readonly View _panelRight;
  private readonly View _panelBottom;
  private readonly View _panelMiddle;

  public LoopPager ViewPager { get => _viewPager; }
  public SlidePanelsGrid DataContext { get; }

  public SlidePanelsGridHost(Context context, SlidePanelsGrid dataContext, BindingScope bindings,
    View left, View top, View right, View bottom, View middle) : base(context) {

    DataContext = dataContext;
    _panelLeft = left;
    _panelTop = top;
    _panelRight = right;
    _panelBottom = bottom;
    _panelMiddle = middle;
    _viewPager = new LoopPager(context, [_panelLeft, _panelMiddle, _panelRight]);

    Orientation = Orientation.Vertical;
    SetBackgroundResource(Resource.Color.c_static_ba);

    _panelTop.BindVisibility(dataContext.PanelTop, nameof(SlidePanel.IsPinned), x => x.IsPinned, bindings);
    _panelBottom.BindVisibility(dataContext.PanelBottom, nameof(SlidePanel.IsPinned), x => x.IsPinned, bindings);

    AddView(top, LPU.LinearMatchWrap());
    AddView(_viewPager, LPU.Linear(LPU.Match, 0, 1f));
    AddView(bottom, LPU.LinearMatchWrap());
  }
}