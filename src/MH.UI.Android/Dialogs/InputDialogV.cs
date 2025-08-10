using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.UI.Dialogs;
using MH.Utils.Extensions;
using System;
using System.ComponentModel;

namespace MH.UI.Android.Dialogs;

public class InputDialogV : GridLayout, IDialogHostContent {
  private readonly ImageView _icon;
  private readonly TextView _message;
  private readonly EditText _answer;
  private bool _disposed;

  public InputDialog? DataContext { get; private set; }

  public InputDialogV(Context context) : base(context) {
    LayoutParameters = new ViewGroup.LayoutParams(
      ViewGroup.LayoutParams.MatchParent,
      ViewGroup.LayoutParams.WrapContent);
    ColumnCount = 2;
    RowCount = 2;

    SetForegroundGravity(GravityFlags.Center);
    SetMinimumWidth(DisplayU.DpToPx(300));
    SetPadding(0, DisplayU.DpToPx(10), 0, DisplayU.DpToPx(10));

    _icon = _createIconView(context);
    _message = _createMessageView(context);
    _answer = _createAnswerView(context);
    _answer.TextChanged += _onAnswerChanged;
    AddView(_icon);
    AddView(_message);
    AddView(_answer);
  }

  public View Bind(Dialog dataContext) {
    if (dataContext is not InputDialog dc) throw new InvalidOperationException();
    _setDataContext(DataContext, dc);
    _icon.SetImageDrawable(Icons.GetIcon(Context, dc.Icon));
    _message.SetText(dc.Message, TextView.BufferType.Normal);
    _answer.SetText(dc.Answer, TextView.BufferType.Normal);
    _answer.Hint = dc.ErrorMessage;
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
        _answer.Background.SetColorFilter(new Color(0x7F, 0xFF, 0x00, 0x00), PorterDuff.Mode.SrcAtop);
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

  private static ImageView _createIconView(Context context) {
    var view = new ImageView(context) {
      LayoutParameters = new GridLayout.LayoutParams(
        rowSpec: GridLayout.InvokeSpec(0, 2),
        columnSpec: GridLayout.InvokeSpec(0)) {
        Width = DisplayU.DpToPx(32),
        Height = DisplayU.DpToPx(32),
        RightMargin = DisplayU.DpToPx(10)
      }
    };
    view.SetForegroundGravity(GravityFlags.Center);

    return view;
  }

  private static TextView _createMessageView(Context context) {
    var view = new TextView(context) {
      LayoutParameters = new GridLayout.LayoutParams(
        rowSpec: GridLayout.InvokeSpec(0),
        columnSpec: GridLayout.InvokeSpec(1)) {
        LeftMargin = DisplayU.DpToPx(5)
      }
    };
    view.SetForegroundGravity(GravityFlags.CenterVertical);

    return view;
  }

  private static EditText _createAnswerView(Context context) {
    var view = new EditText(context) {
      LayoutParameters = new GridLayout.LayoutParams(
        rowSpec: GridLayout.InvokeSpec(1),
        columnSpec: GridLayout.InvokeSpec(1)) {
        LeftMargin = DisplayU.DpToPx(5),
        RightMargin = DisplayU.DpToPx(10),
        TopMargin = DisplayU.DpToPx(5),
        BottomMargin = DisplayU.DpToPx(5)
      }
    };

    return view;
  }
}