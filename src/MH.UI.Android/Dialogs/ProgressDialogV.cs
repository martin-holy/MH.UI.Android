using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Binding;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Dialogs;
using MH.Utils.Disposables;

namespace MH.UI.Android.Dialogs;

public class ProgressDialogV : LinearLayout {
  public ProgressDialogV(Context context, IProgressDialog dataContext, BindingScope bindings) : base(context) {
    Orientation = Orientation.Vertical;
    SetMinimumWidth(DisplayU.DpToPx(300));
    SetPadding(0, DisplayU.DpToPx(10), 0, DisplayU.DpToPx(10));
    SetGravity(GravityFlags.CenterVertical);

    var bar = new ProgressBar(context).BindProgressBar(dataContext, bindings);
    var text = new TextView(context).BindProgressText(dataContext, bindings);

    AddView(bar, LPU.LinearMatchWrap().WithDpMargin(6, 0, 6, 0));
    AddView(text, LPU.LinearWrap().WithDpMargin(0, 0, 0, 6));
  }
}