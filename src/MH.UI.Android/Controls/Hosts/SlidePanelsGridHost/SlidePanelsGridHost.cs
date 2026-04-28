using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils;
using MH.Utils.Disposables;

namespace MH.UI.Android.Controls.Hosts.SlidePanelsGridHost;

public enum TopAndBottomPanelsPlacement { Global, MiddleOnly }

public class SlidePanelsGridHost : FrameLayout {
  private readonly View _panelLeft;
  private readonly SlidePanelHost _panelTop;
  private readonly View _panelRight;
  private readonly SlidePanelHost _panelBottom;
  private readonly View _panelMiddle;
  private readonly FrameLayout _pagerHost;
  private readonly View _middleContent;
  private readonly TopAndBottomPanelsPlacement _gridMode;

  public LoopPager ViewPager { get; }
  public SlidePanelsGrid DataContext { get; }

  public SlidePanelsGridHost(Context context, SlidePanelsGrid dataContext, BindingScope bindings, TopAndBottomPanelsPlacement gridMode,
    View left, View top, View right, View bottom, View middle) : base(context) {
    SetBackgroundResource(Resource.Color.c_static_ba);

    DataContext = dataContext;
    _gridMode = gridMode;
    _middleContent = middle;

    _panelLeft = left;
    _panelTop = new(context, top, dataContext.PanelTop, bindings);
    _panelRight = right;
    _panelBottom = new(context, bottom, dataContext.PanelBottom, bindings);

    if (gridMode == TopAndBottomPanelsPlacement.MiddleOnly) {
      var middleWrapper = new FrameLayout(context);
      middleWrapper.AddView(middle, LPU.FrameMatch());
      middleWrapper.AddView(_panelTop, LPU.Frame(LPU.Match, LPU.Wrap, GravityFlags.Top));
      middleWrapper.AddView(_panelBottom, LPU.Frame(LPU.Match, LPU.Wrap, GravityFlags.Bottom));
      _panelMiddle = middleWrapper;
    }
    else
      _panelMiddle = middle;

    ViewPager = new LoopPager(context, [_panelLeft, _panelMiddle, _panelRight]);

    _pagerHost = new FrameLayout(context);
    _pagerHost.AddView(ViewPager, LPU.FrameMatch());
    AddView(_pagerHost, LPU.FrameMatch());

    if (gridMode == TopAndBottomPanelsPlacement.Global) {
      AddView(_panelTop, LPU.Frame(LPU.Match, LPU.Wrap, GravityFlags.Top));
      AddView(_panelBottom, LPU.Frame(LPU.Match, LPU.Wrap, GravityFlags.Bottom));
    }

    _initPanel(DataContext.PanelTop, bindings);
    _initPanel(DataContext.PanelBottom, bindings);
  }

  private void _initPanel(SlidePanel panel, BindingScope bindings) {
    panel.Bind(nameof(SlidePanel.IsPinned), x => x.IsPinned, _ => _updateLayout(), false).DisposeWith(bindings);
    panel.Bind(nameof(SlidePanel.Size), x => x.Size, _ => _updateLayout(), false).DisposeWith(bindings);
  }

  private void _updateLayout() =>
    _updateMiddleMargins(_gridMode == TopAndBottomPanelsPlacement.MiddleOnly ? _middleContent : _pagerHost);

  private void _updateMiddleMargins(View view) {
    int top = DataContext.PanelTop.IsPinned ? _panelTop.Height : 0;
    int bottom = DataContext.PanelBottom.IsPinned ? _panelBottom.Height : 0;

    if (view.LayoutParameters is not MarginLayoutParams lp) return;
    if (lp.TopMargin == top && lp.BottomMargin == bottom) return;

    lp.TopMargin = top;
    lp.BottomMargin = bottom;

    view.LayoutParameters = lp;
  }
}