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
    _createThisView();
    _icon = new IconView(context, DisplayU.DpToPx(32)).SetMargin(DisplayU.DpToPx(10));
    _message = _createMessageView(context);
    AddView(_icon);
    AddView(_message);
  }

  public View Bind(Dialog dataContext) {
    if (dataContext is not MessageDialog vm) throw new InvalidOperationException();
    _icon.Bind(vm.Icon);
    _message.SetText(vm.Message, TextView.BufferType.Normal);
    return this;
  }

  private void _createThisView() {
    LayoutParameters = new ViewGroup.LayoutParams(
      ViewGroup.LayoutParams.MatchParent,
      ViewGroup.LayoutParams.WrapContent);
    Orientation = Orientation.Horizontal;

    SetMinimumWidth(DisplayU.DpToPx(300));
    SetPadding(0, DisplayU.DpToPx(10), 0, DisplayU.DpToPx(10));
    SetGravity(GravityFlags.CenterVertical);
  }

  private static TextView _createMessageView(Context context) =>
    new(context) {
      LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent) {
        Weight = 1
      }
    };
}