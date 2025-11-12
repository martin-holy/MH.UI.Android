using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Dialogs;

namespace MH.UI.Android.Dialogs;

public class ToggleDialogV : LinearLayout {
  public ToggleDialogV(Context context, ToggleDialog dataContext) : base(context) {
    Orientation = Orientation.Horizontal;
    SetMinimumWidth(DisplayU.DpToPx(300));
    SetPadding(0, DisplayU.DpToPx(10), 0, DisplayU.DpToPx(10));
    SetGravity(GravityFlags.CenterVertical);

    var icon = new IconView(context, Res.IconQuestion);
    var item = new IconTextView(context).BindIcon(dataContext.Item!.Icon).BindText(dataContext.Item.Name);
    var message = new TextView(context) { Text = dataContext.Message };

    var itemAndMessage = new LinearLayout(context) { Orientation = Orientation.Vertical };
    itemAndMessage.AddView(item, new LayoutParams(LPU.Wrap, LPU.Wrap).WithDpMargin(5));
    itemAndMessage.AddView(message, new LayoutParams(LPU.Wrap, LPU.Wrap).WithDpMargin(5, 5, 10, 5));

    AddView(icon, new LayoutParams(DisplayU.DpToPx(32), DisplayU.DpToPx(32)).WithDpMargin(10));
    AddView(itemAndMessage, new LayoutParams(LPU.Wrap, LPU.Wrap, 1f));
  }
}