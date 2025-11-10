using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.UI.Dialogs;
using MH.Utils;
using System;

namespace MH.UI.Android.Dialogs;

public class ProgressDialogV : LinearLayout, IDialogContentV {
  private readonly ProgressBar _progress;
  private readonly TextView _text;
  private IDisposable? _progressBind;
  private IDisposable? _textBind;

  public ProgressDialogV(Context context) : base(context) {
    Orientation = Orientation.Vertical;
    SetMinimumWidth(DisplayU.DpToPx(300));
    SetPadding(0, DisplayU.DpToPx(10), 0, DisplayU.DpToPx(10));
    SetGravity(GravityFlags.CenterVertical);

    _progress = new ProgressBar(context);
    _text = new TextView(context);

    AddView(_progress, new LayoutParams(LPU.Match, LPU.Wrap).WithDpMargin(6, 0, 6, 0));
    AddView(_text, new LayoutParams(LPU.Wrap, LPU.Wrap).WithDpMargin(0, 0, 0, 6));
  }

  public View Bind(Dialog dataContext) {
    _progressBind?.Dispose();
    _textBind?.Dispose();

    if (dataContext is not IProgressDialog vm) throw new InvalidOperationException();

    _progress.Max = vm.ProgressMax;
    _progressBind = _progress.Bind(vm, x => x.ProgressValue, (t, p) => t.Progress = p);
    _textBind = _text.Bind(vm, x => x.ProgressText, (t, p) => t.Text = p);

    return this;
  }
}