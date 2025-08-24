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
    _createThisView();

    _icon = new IconView(context, DisplayU.DpToPx(32))
      .SetMargin(DisplayU.DpToPx(10));

    _item = new IconTextView(context)
      .SetMargin(DisplayU.DpToPx(5));

    _message = new TextView(context)
      .SetMargin(DisplayU.DpToPx(5), DisplayU.DpToPx(5), DisplayU.DpToPx(10), DisplayU.DpToPx(5));

    var itemAndMessage = new LinearLayout(context) {
      Orientation = Orientation.Vertical,
      LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent, 1f)
    };

    itemAndMessage.AddView(_item);
    itemAndMessage.AddView(_message);

    AddView(_icon);
    AddView(itemAndMessage);
  }

  public View Bind(Dialog dataContext) {
    if (dataContext is not ToggleDialog vm) throw new InvalidOperationException();
    _icon.Bind(Res.IconQuestion);
    _item.BindIcon(vm.Item!.Icon).BindText(vm.Item.Name);
    _message.SetText(vm.Message, TextView.BufferType.Normal);
    return this;
  }

  private void _createThisView() {
    Orientation = Orientation.Horizontal;
    LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);

    SetMinimumWidth(DisplayU.DpToPx(300));
    SetPadding(0, DisplayU.DpToPx(10), 0, DisplayU.DpToPx(10));
    SetGravity(GravityFlags.CenterVertical);
  }
}