using Android.Views;
using Android.Widget;
using MH.Utils.BaseClasses;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;

namespace MH.UI.Android.Utils;

public static class BindingU {
  public static IDisposable Bind<TSource, TProp>(
    TSource source,
    Expression<Func<TSource, TProp>> propertyExpression,
    Action<TProp> onChange)
    where TSource : class, INotifyPropertyChanged {

    var path = _getPropertyPath(propertyExpression);

    return new PropertyPathBinding<TSource, TProp>(source, path, onChange);
  }

  public static IDisposable Bind(INotifyCollectionChanged source, Action<NotifyCollectionChangedEventArgs> onChange) {
    var weakSource = new WeakReference<INotifyCollectionChanged>(source);

    void handler(object? o, NotifyCollectionChangedEventArgs e) {
      if (weakSource.TryGetTarget(out _))
        onChange(e);
      else if (o is INotifyCollectionChanged col)
        col.CollectionChanged -= handler;
    }

    source.CollectionChanged += handler;
    onChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

    return new Subscription(() => source.CollectionChanged -= handler);
  }

  public static CommandBinding Bind(View view, ICommand command, bool useCommandIcon = true, bool useCommandText = true) =>
    Bind(view, command, null, useCommandIcon, useCommandText);

  public static CommandBinding Bind(View view, ICommand command, object? parameter, bool useCommandIcon = true, bool useCommandText = true) {
    if (command is not RelayCommandBase cmd)
      return new(view, command, parameter);

    if (useCommandIcon && view is ImageView imageView && Icons.GetIcon(view.Context, cmd.Icon) is { } icon)
      imageView.SetImageDrawable(icon);

    if (useCommandText && view is TextView textView)
      textView.Text = cmd.Text;

    return new(view, command, parameter);
  }

  private static string[] _getPropertyPath<TSource, TProp>(Expression<Func<TSource, TProp>> expression) {
    var members = new Stack<string>();
    Expression? exp = expression.Body;

    while (exp is MemberExpression member) {
      members.Push(member.Member.Name);
      exp = member.Expression;
    }

    return [.. members];
  }

  private class PropertyPathBinding<TSource, TProp> : IDisposable where TSource : class, INotifyPropertyChanged {
    private readonly Action<TProp> _onChange;
    private readonly string[] _path;
    private WeakReference<INotifyPropertyChanged>? _currentRef;
    private PropertyChangedEventHandler? _handler;

    public PropertyPathBinding(TSource source, string[] path, Action<TProp> onChange) {
      _path = path;
      _onChange = onChange;
      _attach(source, 0);
      _fireInitial(source);
    }

    private void _attach(INotifyPropertyChanged source, int index) {
      if (index >= _path.Length) return;

      _currentRef = new WeakReference<INotifyPropertyChanged>(source);

      _handler = (o, e) => {
        if (!_currentRef.TryGetTarget(out var target)) {
          if (o is INotifyPropertyChanged npc)
            npc.PropertyChanged -= _handler!;

          return;
        }

        if (e.PropertyName != _path[index] && !string.IsNullOrEmpty(e.PropertyName)) return;

        if (index < _path.Length - 1 && _getPropertyValue(target, _path[index]) is INotifyPropertyChanged next)
          _attach(next, index + 1);
        else if (index == _path.Length - 1)
          _fire(target);
      };

      source.PropertyChanged += _handler;

      if (index < _path.Length - 1 && _getPropertyValue(source, _path[index]) is INotifyPropertyChanged next)
        _attach(next, index + 1);
    }

    private void _fire(INotifyPropertyChanged source) {
      var value = _resolvePath(source, _path);
      _onChange((TProp)value!);
    }

    private void _fireInitial(TSource source) {
      var value = _resolvePath(source, _path);
      _onChange((TProp)value!);
    }

    public void Dispose() {
      if (_currentRef != null && _handler != null && _currentRef.TryGetTarget(out var target))
        target.PropertyChanged -= _handler;

      _handler = null;
      _currentRef = null;
    }
  }

  private static object? _resolvePath(object source, string[] path) {
    object? current = source;
    foreach (var prop in path) {
      if (current == null) return null;
      var pi = current.GetType().GetProperty(prop);
      current = pi?.GetValue(current);
    }
    return current;
  }

  private static object? _getPropertyValue(object source, string prop) {
    var pi = source.GetType().GetProperty(prop);
    return pi?.GetValue(source);
  }

  private class Subscription(Action _unsubscribe) : IDisposable {
    public void Dispose() => _unsubscribe?.Invoke();
  }
}