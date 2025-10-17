using Android.Text;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.Utils;
using MH.Utils.BaseClasses;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;

namespace MH.UI.Android.Utils;

public static class BindingU {
  public static CommandBinding Bind(this View view, ICommand command, object? parameter, bool useCommandIcon = true, bool useCommandText = true) {
    if (command is not RelayCommandBase cmd)
      return new(view, command, parameter);

    if (useCommandIcon) {
      switch (view) {
        case ImageView imageView:
          if (Icons.GetIcon(view.Context, cmd.Icon) is { } icon)
            imageView.SetImageDrawable(icon);
          break;
        case CompactIconTextButton citBtn:
          citBtn.Icon.Bind(cmd.Icon);
          break;
      }
    }

    if (useCommandText && view is TextView textView)
      textView.Text = cmd.Text;

    return new(view, command, parameter);
  }

  public static CommandBinding Bind(this View view, ICommand command, bool useCommandIcon = true, bool useCommandText = true)
    => Bind(view, command, null, useCommandIcon, useCommandText);

  public static T WithCommand<T>(this T view, ICommand command, bool useCommandIcon = true, bool useCommandText = true) where T : View {
    view.Bind(command, null, useCommandIcon, useCommandText);
    return view;
  }

  public static T WithCommand<T>(this T view, ICommand command, object? parameter, bool useCommandIcon = true, bool useCommandText = true) where T : View {
    view.Bind(command, parameter, useCommandIcon, useCommandText);
    return view;
  }

  public static TextView BindText<TSource, TProp>(
    this TextView textView,
    TSource source,
    Expression<Func<TSource, TProp>> property,
    Func<TProp, string> formatter)
    where TSource : class, INotifyPropertyChanged {

    new ViewBinder<TextView, string>(
      textView,
      (v, val) => v.Text = formatter((TProp)Convert.ChangeType(val, typeof(TProp))!))
      .Bind(source, property);

    return textView;
  }

  public static EditText BindText<TSource, TProp>(this EditText editText, TSource source, Expression<Func<TSource, TProp>> property)
    where TSource : class, INotifyPropertyChanged {

    EventHandler<TextChangedEventArgs>? handler = null;

    new ViewBinder<EditText, string>(
      editText,
      eh => {
        handler = (s, e) => eh(s, e.Text?.ToString() ?? string.Empty);
        editText.TextChanged += handler;
      },
      eh => { if (handler != null) editText.TextChanged -= handler; },
      (v, val) => { if (!val.Equals(v.Text)) v.Text = val; })
      .Bind(source, property);

    return editText;
  }

  public static CheckBox BindChecked<TSource, TProp>(this CheckBox checkBox, TSource source, Expression<Func<TSource, TProp>> property)
    where TSource : class, INotifyPropertyChanged {

    EventHandler<CompoundButton.CheckedChangeEventArgs>? handler = null;

    new ViewBinder<CheckBox, bool>(
      checkBox,
      eh => {
        handler = (s, e) => eh(s, e.IsChecked);
        checkBox.CheckedChange += handler;
      },
      eh => { if (handler != null) checkBox.CheckedChange -= handler; },
      (v, val) => { if (val != v.Checked) v.Checked = val; })
      .Bind(source, property);

    return checkBox;
  }
}