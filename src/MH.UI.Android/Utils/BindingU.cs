using Android.Views;
using Android.Widget;
using MH.Utils.BaseClasses;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;

namespace MH.UI.Android.Utils;

public static class BindingU {
  public static IDisposable Bind<TSource, TProp>(
    TSource source,
    Expression<Func<TSource, TProp>> propertyExpression,
    Action<TProp> onChange,
    object? target = null)
    where TSource : class, INotifyPropertyChanged {

    if (propertyExpression.Body is not MemberExpression member)
      throw new ArgumentException("Expression must be a property access.", nameof(propertyExpression));

    var propName = member.Member.Name;
    var getter = propertyExpression.Compile();
    var weakTarget = target == null ? null : new WeakReference<object>(target);
    var weakSource = new WeakReference<TSource>(source);

    void handler(object? sender, PropertyChangedEventArgs e) {
      if (weakTarget?.TryGetTarget(out _) == false) {
        if (sender is INotifyPropertyChanged npc)
          npc.PropertyChanged -= handler;

        return;
      }

      if ((string.IsNullOrEmpty(e.PropertyName) || propName.Equals(e.PropertyName, StringComparison.OrdinalIgnoreCase))
        && weakSource.TryGetTarget(out var s))
        onChange(getter(s));
    }

    source.PropertyChanged += handler;

    if (weakTarget == null || weakTarget.TryGetTarget(out _))
      onChange(getter(source));

    return new Subscription(() => {
      if (weakSource.TryGetTarget(out var s))
        s.PropertyChanged -= handler;
    });
  }

  public static IDisposable Bind(
    INotifyCollectionChanged source,
    Action<NotifyCollectionChangedEventArgs> onChange,
    object? target = null) {

    var weakTarget = target == null ? null : new WeakReference<object>(target);
    var weakSource = new WeakReference<INotifyCollectionChanged>(source);

    void handler(object? sender, NotifyCollectionChangedEventArgs e) {
      if (weakTarget?.TryGetTarget(out _) == false) {
        if (sender is INotifyCollectionChanged col)
          col.CollectionChanged -= handler;

        return;
      }

      onChange(e);
    }

    source.CollectionChanged += handler;

    if (weakTarget == null || weakTarget.TryGetTarget(out _))
      onChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

    return new Subscription(() => {
      if (weakSource.TryGetTarget(out var s))
        s.CollectionChanged -= handler;
    });
  }

  public static CommandBinding Bind(View view, ICommand command, bool useCommandIcon = true, bool useCommandText = true)
    => Bind(view, command, null, useCommandIcon, useCommandText);

  public static CommandBinding Bind(View view, ICommand command, object? parameter, bool useCommandIcon = true, bool useCommandText = true) {
    if (command is not RelayCommandBase cmd)
      return new(view, command, parameter);

    if (useCommandIcon && view is ImageView imageView && Icons.GetIcon(view.Context, cmd.Icon) is { } icon)
      imageView.SetImageDrawable(icon);

    if (useCommandText && view is TextView textView)
      textView.Text = cmd.Text;

    return new(view, command, parameter);
  }

  private class Subscription(Action _unsubscribe) : IDisposable {
    public void Dispose() => _unsubscribe();
  }
}