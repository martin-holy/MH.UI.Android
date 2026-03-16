using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;
using MH.Utils.Extensions;
using System;
using System.Windows.Input;

namespace MH.UI.Android.Binding;

public sealed class CommandBinding : IDisposable {
  private readonly View _view;
  private ICommand? _command;
  private object? _parameter;

  public object? Parameter {
    get => _parameter;
    set {
      _parameter = value;
      _updateEnabledState();
    }
  }

  public CommandBinding(View view) {
    _view = view;
    _view.Click += _onClick;
  }

  public CommandBinding Bind(ICommand command, bool useCommandIcon = true, bool useCommandText = true) =>
    Bind(command, null, useCommandIcon, useCommandText);

  public CommandBinding Bind(ICommand command, object? parameter, bool useCommandIcon = true, bool useCommandText = true) {
    Unbind();

    _command = command;
    _parameter = parameter;
    _command.CanExecuteChanged += _onCanExecuteChanged;

    _setIconAndText(useCommandIcon, useCommandText);
    _updateEnabledState();

    return this;
  }

  public void Unbind() {
    if (_command != null)
      _command.CanExecuteChanged -= _onCanExecuteChanged;

    _command = null;
    _parameter = null;
  }

  private void _setIconAndText(bool useCommandIcon, bool useCommandText) {
    if (_command is not RelayCommandBase cmd) return;

    if (useCommandIcon) {
      switch (_view) {
        case ImageView imageView:
          if (IconU.GetIcon(_view.Context, cmd.Icon) is { } icon)
            imageView.SetImageDrawable(icon);
          break;
        case CompactIconTextButton citBtn:
          citBtn.Icon.Bind(cmd.Icon);
          break;
      }
    }

    if (useCommandText && _view is TextView textView)
      textView.Text = cmd.Text;
  }

  private void _onClick(object? sender, EventArgs e) {
    _command?.ExecuteIfCan(_parameter);
  }

  private void _onCanExecuteChanged(object? sender, EventArgs e) {
    _updateEnabledState();
  }

  private void _updateEnabledState() {
    if (_view.IsAttachedToWindow)
      _view.Enabled = _command?.CanExecute(_parameter) ?? false;
  }

  public void Dispose() {
    Unbind();
    _view.Click -= _onClick;
  }
}