using Android.Text;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.Utils;
using MH.Utils.BaseClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    string propertyName,
    Func<TSource, TProp> getter,
    Func<TProp, string> formatter,
    out IDisposable? binder)
    where TSource : class, INotifyPropertyChanged {

    binder = new ViewBinder<TextView, TSource, TProp, string>(textView, source, propertyName, getter,
      (target, v) => target.Text = formatter((TProp)Convert.ChangeType(v, typeof(TProp))!));

    return textView;
  }

  public static EditText BindText<TSource, TProp>(
    this EditText editText,
    TSource source,
    string propertyName,
    Func<TSource, TProp> getter,
    Action<TSource, TProp> setter,
    out IDisposable? binder)
    where TSource : class, INotifyPropertyChanged {

    EventHandler<TextChangedEventArgs>? handler = null;

    binder = new ViewBinder<EditText, TSource, TProp, string>(editText, source, propertyName, getter, setter,
      (target, v) => { if (!v.Equals(target.Text)) target.Text = v; },
      eh => {
        handler = (s, e) => eh(s, e.Text?.ToString() ?? string.Empty);
        editText.TextChanged += handler;
      },
      eh => { if (handler != null) editText.TextChanged -= handler; });

    return editText;
  }

  public static CheckBox BindChecked<TSource, TProp>(
    this CheckBox checkBox,
    TSource source,
    string propertyName,
    Func<TSource, TProp> getter,
    Action<TSource, TProp> setter,
    out IDisposable? binder)
    where TSource : class, INotifyPropertyChanged {

    EventHandler<CompoundButton.CheckedChangeEventArgs>? handler = null;

    binder = new ViewBinder<CheckBox, TSource, TProp, bool>(checkBox, source, propertyName, getter, setter,
      (target, v) => { if (v != target.Checked) target.Checked = v; },
      eh => {
        handler = (s, e) => eh(s, e.IsChecked);
        checkBox.CheckedChange += handler;
      },
      eh => { if (handler != null) checkBox.CheckedChange -= handler; });

    return checkBox;
  }

  public static Slider BindProgress<TSource, TProp>(
    this Slider slider,
    TSource source,
    string propertyName,
    Func<TSource, TProp> getter,
    Action<TSource, TProp> setter,
    out IDisposable? binder)
    where TSource : class, INotifyPropertyChanged {

    EventHandler<SeekBar.ProgressChangedEventArgs>? handler = null;

    binder = new ViewBinder<Slider, TSource, TProp, double>(slider, source, propertyName, getter, setter,
      (target, v) => target.Progress = (int)Math.Round((v - target.MinD) * target.Scale),
      eh => {
        handler = (s, e) => {
          if (!e.FromUser) return;
          var sl = (Slider)s!;
          var value = e.Progress / sl.Scale + sl.MinD;
          var snapped = Math.Round(value / sl.TickFrequency) * sl.TickFrequency;
          var snappedProgress = (int)Math.Round((snapped - sl.MinD) * sl.Scale);
          if (snappedProgress != e.Progress) sl.Progress = snappedProgress;
          eh(s, snapped);
        };
        slider.ProgressChanged += handler;
      },
      eh => { if (handler != null) slider.ProgressChanged -= handler; });

    return slider;
  }

  public static RadioGroup BindChecked<TSource, TEnum>(
    this RadioGroup radioGroup,
    TSource source,
    string propertyName,
    Func<TSource, TEnum> getter,
    Action<TSource, TEnum> setter,
    TEnum[] values,
    out IDisposable? binder)
    where TSource : class, INotifyPropertyChanged
    where TEnum : struct, Enum {

    EventHandler<RadioGroup.CheckedChangeEventArgs>? handler = null;

    binder = new ViewBinder<RadioGroup, TSource, TEnum, TEnum>(radioGroup, source, propertyName, getter, setter,
      (target, v) => {
        for (int i = 0; i < values.Length && i < radioGroup.ChildCount; i++) {
          if (EqualityComparer<TEnum>.Default.Equals(values[i], v)) {
            if (radioGroup.GetChildAt(i) is RadioButton rb && !rb.Checked) target.Check(rb.Id);
            break;
          }
        }
      },
      eh => {
        handler = (s, e) => {
          if (radioGroup.FindViewById<RadioButton>(e.CheckedId) is not { } checkedButton) return;

          for (int i = 0; i < values.Length; i++) {
            var rb = radioGroup.GetChildAt(i) as RadioButton;
            if (rb?.Id == checkedButton.Id) {
              eh(s, values[i]);
              break;
            }
          }
        };
        radioGroup.CheckedChange += handler;
      },
      eh => {
        if (handler != null) radioGroup.CheckedChange -= handler;
      });

    return radioGroup;
  }

  public static Spinner BindSelected<TSource, TProp, TKey>(
    this Spinner spinner,
    TSource source,
    string propertyName,
    Func<TSource, TProp> getter,
    Action<TSource, TProp> setter,
    KeyValuePair<TKey, string>[] map,
    out IDisposable? binder)
    where TSource : class, INotifyPropertyChanged {

    EventHandler<AdapterView.ItemSelectedEventArgs>? handler = null;

    var adapter = new ArrayAdapter<string>(spinner.Context!, Resource.Layout.spinner_item, [.. map.Select(x => x.Value)]);
    adapter.SetDropDownViewResource(Resource.Layout.spinner_dropdown_item);
    spinner.Adapter = adapter;

    binder = new ViewBinder<Spinner, TSource, TProp, TKey>(spinner, source, propertyName, getter, setter,
      (target, v) => {
        var index = Array.FindIndex(map, kvp => Equals(kvp.Key, v));
        if (index >= 0 && target.SelectedItemPosition != index)
          target.SetSelection(index);
      },
      eh => {
        handler = (s, e) => eh(s, map[e.Position].Key);
        spinner.ItemSelected += handler;
      },
      eh => { if (handler != null) spinner.ItemSelected -= handler; });

    return spinner;
  }

  public static TTarget BindVisibility<TTarget, TSource>(
    this TTarget view,
    TSource source,
    string propertyName,
    Func<TSource, bool> getter)
    where TTarget : View
    where TSource : class, INotifyPropertyChanged {

    view.Bind(source, propertyName, getter, (t, p) => t.Visibility = p ? ViewStates.Visible : ViewStates.Gone);
    return view;
  }
}