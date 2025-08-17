using Android.Views;
using MH.Utils.Extensions;
using System;
using System.Windows.Input;

namespace MH.UI.Android.Utils;

public sealed class CommandBinding : IDisposable {
  private readonly View _view;
  private readonly ICommand _command;
  private object? _parameter;
  private bool _disposed;

  public object? Parameter {
    get => _parameter;
    set {
      _parameter = value;
      _updateEnabledState();
    }
  }

  public CommandBinding(View view, ICommand command) {
    _view = view;
    _view.Click += _onClick;
    _command = command;
    _updateEnabledState();
  }

  public CommandBinding(View view, ICommand command, object? parameter) : this(view, command) {
    _parameter = parameter;
  }

  private void _onClick(object? sender, EventArgs e) =>
    _command.ExecuteIfCan(_parameter);

  private void _updateEnabledState() =>
    _view.Enabled = _command.CanExecute(_parameter);

  public void Dispose() {
    if (_disposed) return;
    _view.Click -= _onClick;
    _disposed = true;
  }
}