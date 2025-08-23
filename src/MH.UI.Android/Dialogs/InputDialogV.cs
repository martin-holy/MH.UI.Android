using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.UI.Dialogs;
using MH.Utils.Extensions;
using System;
using System.ComponentModel;

namespace MH.UI.Android.Dialogs;

public class InputDialogV : GridLayout, IDialogContentV {
  private readonly IconView _icon;
  private readonly TextView _message;
  private readonly EditText _answer;
  private bool _disposed;

  public InputDialog? DataContext { get; private set; }

  public InputDialogV(Context context) : base(context) {
    _createThisView();

    _icon = new IconView(context, DisplayU.DpToPx(32))
      .SetGridPosition(InvokeSpec(0, 2), InvokeSpec(0), GravityFlags.Center)
      .SetMargin(DisplayU.DpToPx(10));

    _message = new TextView(context)
      .SetGridPosition(InvokeSpec(0), InvokeSpec(1, 1f), GravityFlags.CenterVertical)
      .SetMargin(DisplayU.DpToPx(5));

    _answer = new EditText(context)
      .SetGridPosition(InvokeSpec(1), InvokeSpec(1, 1f))
      .SetMargin(DisplayU.DpToPx(5), DisplayU.DpToPx(5), DisplayU.DpToPx(10), DisplayU.DpToPx(5));
    _answer.TextChanged += _onAnswerChanged;

    AddView(_icon);
    AddView(_message);
    AddView(_answer);
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
        _answer.Background.SetColorFilter(new Color(0x7F, 0xFF, 0x00, 0x00), PorterDuff.Mode.SrcAtop); // TODO get color from resources
      else
        _answer.Background.ClearColorFilter();
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
    LayoutParameters = new ViewGroup.LayoutParams(
      ViewGroup.LayoutParams.MatchParent,
      ViewGroup.LayoutParams.WrapContent);
    ColumnCount = 2;
    RowCount = 2;
    
    SetMinimumWidth(DisplayU.DpToPx(300));
    SetPadding(0, DisplayU.DpToPx(10), 0, DisplayU.DpToPx(10));
  }
}