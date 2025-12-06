using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils;

namespace MH.UI.Android.Controls;

public class SlidePanelsGridHost : LinearLayout {
  private readonly LoopPager _viewPager;
  private readonly View _panelLeft;
  private readonly View _panelTop;
  private readonly View _panelRight;
  private readonly View _panelBottom;
  private readonly View _panelMiddle;

  public LoopPager ViewPager { get => _viewPager; }
  public SlidePanelsGrid DataContext { get; }

  public SlidePanelsGridHost(Context context, SlidePanelsGrid dataContext, View left, View top, View right, View bottom, View middle) : base(context) {
    DataContext = dataContext;
    _panelLeft = left;
    _panelTop = top;
    _panelRight = right;
    _panelBottom = bottom;
    _panelMiddle = middle;
    _viewPager = new LoopPager(context, [_panelLeft, _panelMiddle, _panelRight]);

    Orientation = Orientation.Vertical;
    SetBackgroundResource(Resource.Color.c_static_ba);

    this.Bind(dataContext.PanelTop, nameof(SlidePanel.IsPinned), x => x.IsPinned, (v, p) => v._panelTop.Visibility = p ? ViewStates.Visible : ViewStates.Gone);
    this.Bind(dataContext.PanelBottom, nameof(SlidePanel.IsPinned), x => x.IsPinned, (v, p) => v._panelBottom.Visibility = p ? ViewStates.Visible : ViewStates.Gone);

    AddView(top, new LayoutParams(LPU.Match, LPU.Wrap));
    AddView(_viewPager, new LayoutParams(LPU.Match, 0, 1f));
    AddView(bottom, new LayoutParams(LPU.Match, LPU.Wrap));
  }
}