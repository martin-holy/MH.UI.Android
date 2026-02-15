using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;

namespace MH.UI.Android.Controls;

public class PopupSlider : FrameLayout {
  public PopupWindow Popup { get; }
  public Slider Slider { get; }

  public PopupSlider(Context context, double minD, double maxD, double tickFrequency, View content) : base(context) {
    Slider = new(context, minD, maxD, tickFrequency);
    Popup = new PopupWindow(Slider, DisplayU.DpToPx(160), LPU.Wrap, true);

    AddView(content, new LayoutParams(LPU.Wrap, LPU.Wrap));

    Slider.StopTrackingTouch += (_, _) => Popup.Dismiss();
    content.WithClickAction(this, (t, p) => t.Popup.ShowAsDropDown(p));
  }
}