using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.UI.Dialogs;
using System;

namespace MH.UI.Android.Dialogs;

public class ToggleDialogV : LinearLayout, IDialogContentV {
  private readonly IconView _icon;
  private readonly IconTextView _item;
  private readonly TextView _message;

  public ToggleDialogV(Context context) : base(context) {
    Orientation = Orientation.Horizontal;
    SetMinimumWidth(DisplayU.DpToPx(300));
    SetPadding(0, DisplayU.DpToPx(10), 0, DisplayU.DpToPx(10));
    SetGravity(GravityFlags.CenterVertical);

    _icon = new IconView(context);
    _item = new IconTextView(context);
    _message = new TextView(context);
    var itemAndMessage = new LinearLayout(context) { Orientation = Orientation.Vertical };

    itemAndMessage.AddView(_item, new LayoutParams(LPU.Wrap, LPU.Wrap).WithMargin(DisplayU.DpToPx(5)));
    itemAndMessage.AddView(_message, new LayoutParams(LPU.Wrap, LPU.Wrap)
      .WithMargin(DisplayU.DpToPx(5), DisplayU.DpToPx(5), DisplayU.DpToPx(10), DisplayU.DpToPx(5)));

    AddView(_icon, new LayoutParams(DisplayU.DpToPx(32), DisplayU.DpToPx(32)).WithMargin(DisplayU.DpToPx(10)));
    AddView(itemAndMessage, new LayoutParams(LPU.Wrap, LPU.Wrap, 1f));
  }

  public View Bind(Dialog dataContext) {
    if (dataContext is not ToggleDialog vm) throw new InvalidOperationException();
    _icon.Bind(Res.IconQuestion);
    _item.BindIcon(vm.Item!.Icon).BindText(vm.Item.Name);
    _message.Text = vm.Message;
    return this;
  }
}