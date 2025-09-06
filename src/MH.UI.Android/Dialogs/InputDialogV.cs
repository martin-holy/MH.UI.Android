using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using MH.UI.Android.Controls;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.UI.Dialogs;
using MH.Utils.Extensions;
using System;
using System.ComponentModel;

namespace MH.UI.Android.Dialogs;

public sealed class InputDialogV : LinearLayout, IDialogContentV {
  private readonly IconView _icon;
  private readonly TextView _message;
  private readonly EditText _answer;
  private bool _disposed;

  public InputDialog? DataContext { get; private set; }

  public InputDialogV(Context context) : base(context) {
    _createThisView();

    _icon = new IconView(context, DisplayU.DpToPx(32))
      .SetMargin(DisplayU.DpToPx(10));

    _message = new TextView(context)
      .SetMargin(DisplayU.DpToPx(5));

    _answer = new EditText(context) {
      LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
    };
    _answer.SetMargin(DisplayU.DpToPx(5), DisplayU.DpToPx(5), DisplayU.DpToPx(10), DisplayU.DpToPx(5));
    _answer.TextChanged += _onAnswerChanged;

    var messageAndAnswer = new LinearLayout(context) {
      Orientation = Orientation.Vertical,
      LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent, 1f)
    };

    messageAndAnswer.AddView(_message);
    messageAndAnswer.AddView(_answer);

    AddView(_icon);
    AddView(messageAndAnswer);
  }

  public View Bind(Dialog dataContext) {
    if (dataContext is not InputDialog vm) throw new InvalidOperationException();
    _setDataContext(DataContext, vm);
    _icon.Bind(vm.Icon);
    _message.SetText(vm.Message, TextView.BufferType.Normal);
    _answer.SetText(vm.Answer, TextView.BufferType.Normal);
    _answer.Hint = vm.ErrorMessage;
    _answer.RequestFocus();
    return this;
  }

  private void _setDataContext(InputDialog? oldValue, InputDialog? newValue) {
    if (oldValue != null) oldValue.PropertyChanged -= _onDataContextPropertyChanged;
    if (newValue != null) newValue.PropertyChanged += _onDataContextPropertyChanged;
    DataContext = newValue;
  }

  private void _onDataContextPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    if (e.Is(nameof(InputDialog.Error)) && _answer.Background != null) {
      if (DataContext!.Error)
        _answer.Background!.SetColorFilter(new PorterDuffColorFilter(
          new Color(ContextCompat.GetColor(Context, Resource.Color.c_input_error)),
          PorterDuff.Mode.SrcAtop!));
      else
        _answer.Background!.ClearColorFilter();
    }
  }

  private void _onAnswerChanged(object? sender, TextChangedEventArgs e) {
    if (DataContext == null) return;
    DataContext.Answer = e.Text?.ToString();
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _answer.TextChanged -= _onAnswerChanged;
      _setDataContext(DataContext, null);
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  private void _createThisView() {
    Orientation = Orientation.Horizontal;
    LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

    SetMinimumWidth(DisplayU.DpToPx(300));
    SetPadding(0, DisplayU.DpToPx(10), 0, DisplayU.DpToPx(10));
    SetGravity(GravityFlags.CenterVertical);
  }
}