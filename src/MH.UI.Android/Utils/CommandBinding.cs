using Android.Views;
using MH.Utils.Extensions;
using System;
using System.Windows.Input;

namespace MH.UI.Android.Utils;

public sealed class CommandBinding : IDisposable {
  private readonly WeakReference<View> _viewRef;
  private readonly ICommand _command;
  private object? _parameter;

  public object? Parameter {
    get => _parameter;
    set {
      _parameter = value;
      _updateEnabledState();
    }
  }

  public CommandBinding(View view, ICommand command) {
    _viewRef = new(view);
    view.Click += _onClick;
    _command = command;
    _command.CanExecuteChanged += _onCanExecuteChanged;
    _updateEnabledState();
  }

  public CommandBinding(View view, ICommand command, object? parameter) : this(view, command) {
    _parameter = parameter;
  }

  private void _onClick(object? sender, EventArgs e) {
    if (_viewRef.TryGetTarget(out _))
      _command.ExecuteIfCan(_parameter);
    else
      Dispose();
  }

  private void _onCanExecuteChanged(object? sender, EventArgs e) =>
    _updateEnabledState();

  private void _updateEnabledState() {
    if (_viewRef.TryGetTarget(out var view))
      view.Enabled = _command.CanExecute(_parameter);
    else
      Dispose();
  }

  public void Dispose() {
    if (_viewRef.TryGetTarget(out var view))
      view.Click -= _onClick;

    _command.CanExecuteChanged -= _onCanExecuteChanged;
  }
}