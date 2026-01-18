using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Dialogs;

namespace MH.UI.Android.Dialogs;

public class ProgressDialogV : LinearLayout {
  public ProgressDialogV(Context context, IProgressDialog dataContext) : base(context) {
    Orientation = Orientation.Vertical;
    SetMinimumWidth(DisplayU.DpToPx(300));
    SetPadding(0, DisplayU.DpToPx(10), 0, DisplayU.DpToPx(10));
    SetGravity(GravityFlags.CenterVertical);

    AddView(new ProgressBar(context).BindProgressBar(dataContext, out var _),
      new LayoutParams(LPU.Match, LPU.Wrap).WithDpMargin(6, 0, 6, 0));
    AddView(new TextView(context).BindProgressText(dataContext, out var _),
      new LayoutParams(LPU.Wrap, LPU.Wrap).WithDpMargin(0, 0, 0, 6));
  }
}