using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Dialogs;

namespace MH.UI.Android.Dialogs;

public class MessageDialogV : LinearLayout {
  public MessageDialogV(Context context, MessageDialog dataContext) : base(context) {
    Orientation = Orientation.Horizontal;
    SetMinimumWidth(DisplayU.DpToPx(300));
    SetPadding(0, DisplayU.DpToPx(10), 0, DisplayU.DpToPx(10));
    SetGravity(GravityFlags.CenterVertical);

    AddView(new IconView(context).Bind(dataContext.Icon),
      new LayoutParams(DisplayU.DpToPx(32), DisplayU.DpToPx(32)).WithDpMargin(10));
    AddView(new TextView(context) { Text = dataContext.Message }, new LayoutParams(0, LPU.Wrap, 1f));
  }
}