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
    progress.Bind(dataContext, nameof(dataContext.ProgressValue), x => x.ProgressValue, (s, v) => s.Progress = v);

    AddView(progress, new LayoutParams(LPU.Match, LPU.Wrap).WithDpMargin(6, 0, 6, 0));
    AddView(new TextView(context)
      .BindText(dataContext, nameof(IProgressDialog.ProgressText), x => x.ProgressText, x => x ?? string.Empty, out var _),
      new LayoutParams(LPU.Wrap, LPU.Wrap).WithDpMargin(0, 0, 0, 6));
  }
}