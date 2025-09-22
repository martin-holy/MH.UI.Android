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

public class MessageDialogV : LinearLayout, IDialogContentV {
  private readonly IconView _icon;
  private readonly TextView _message;

  public MessageDialogV(Context context) : base(context) {
    Orientation = Orientation.Horizontal;
    SetMinimumWidth(DisplayU.DpToPx(300));
    SetPadding(0, DisplayU.DpToPx(10), 0, DisplayU.DpToPx(10));
    SetGravity(GravityFlags.CenterVertical);

    _icon = new IconView(context);
    _message = new TextView(context);

    AddView(_icon, new LayoutParams(DisplayU.DpToPx(32), DisplayU.DpToPx(32)).WithMargin(DisplayU.DpToPx(10)));
    AddView(_message, new LayoutParams(0, LPU.Wrap, 1f));
  }

  public View Bind(Dialog dataContext) {
    if (dataContext is not MessageDialog vm) throw new InvalidOperationException();
    _icon.Bind(vm.Icon);
    _message.Text = vm.Message;
    return this;
  }
}