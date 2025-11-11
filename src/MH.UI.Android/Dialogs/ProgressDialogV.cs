using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Dialogs;
using MH.Utils;

namespace MH.UI.Android.Dialogs;

public class ProgressDialogV : LinearLayout {
  public ProgressDialogV(Context context, IProgressDialog dataContext) : base(context) {
    Orientation = Orientation.Vertical;
    SetMinimumWidth(DisplayU.DpToPx(300));
    SetPadding(0, DisplayU.DpToPx(10), 0, DisplayU.DpToPx(10));
    SetGravity(GravityFlags.CenterVertical);

    var progress = new ProgressBar(context) { Max = dataContext.ProgressMax };
    var text = new TextView(context);

    progress.Bind(dataContext, x => x.ProgressValue, (t, p) => t.Progress = p);
    text.Bind(dataContext, x => x.ProgressText, (t, p) => t.Text = p);

    AddView(progress, new LayoutParams(LPU.Match, LPU.Wrap).WithDpMargin(6, 0, 6, 0));
    AddView(text, new LayoutParams(LPU.Wrap, LPU.Wrap).WithDpMargin(0, 0, 0, 6));
  }
}