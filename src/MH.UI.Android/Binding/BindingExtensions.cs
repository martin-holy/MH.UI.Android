using Android.Text;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.UI.Dialogs;
using MH.Utils;
using MH.Utils.Disposables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace MH.UI.Android.Binding;

public static class BindingExtensions {
  public static T WithClickCommand<T>(
    this T view,
    ICommand command,
    BindingScope bindings,
    object? parameter = null,
    bool useCommandIcon = true,
    bool useCommandText = true) where T : View {

    bindings.Add(new CommandBinding(view).Bind(command, parameter, useCommandIcon, useCommandText));

    return view;
  }

  public static TextView BindText<TSource, TProp>(
    this TextView textView,
    TSource source,
    string propertyName,
    Func<TSource, TProp> getter,
    Func<TProp, string> formatter,
    BindingScope bindings)
    where TSource : class, INotifyPropertyChanged {

    bindings.Add(new ViewBinder<TSource, TProp, string>(source, propertyName, getter,
      v => textView.Text = formatter((TProp)Convert.ChangeType(v, typeof(TProp))!)));

    return textView;
  }

  public static EditText BindText<TSource, TProp>(
    this EditText editText,
    TSource source,
    string propertyName,
    Func<TSource, TProp> getter,
    Action<TSource, TProp> setter,
    BindingScope bindings)
    where TSource : class, INotifyPropertyChanged {

    EventHandler<TextChangedEventArgs>? handler = null;

    bindings.Add(new ViewBinder<TSource, TProp, string>(source, propertyName, getter, setter,
      v => { if (!string.Equals(editText.Text, v)) editText.Text = v; },
      eh => {
        handler = (s, e) => eh(s, e.Text?.ToString() ?? string.Empty);
        editText.TextChanged += handler;
      },
      eh => { if (handler != null) editText.TextChanged -= handler; }));

    return editText;
  }

  public static CheckBox BindChecked<TSource, TProp>(
    this CheckBox checkBox,
    TSource source,
    string propertyName,
    Func<TSource, TProp> getter,
    Action<TSource, TProp> setter,
    BindingScope bindings)
    where TSource : class, INotifyPropertyChanged {

    EventHandler<CompoundButton.CheckedChangeEventArgs>? handler = null;

    bindings.Add(new ViewBinder<TSource, TProp, bool>(source, propertyName, getter, setter,
      v => { if (v != checkBox.Checked) checkBox.Checked = v; },
      eh => {
        handler = (s, e) => eh(s, e.IsChecked);
        checkBox.CheckedChange += handler;
      },
      eh => { if (handler != null) checkBox.CheckedChange -= handler; }));

    return checkBox;
  }

  public static IconToggleButton BindToggled<TSource, TProp>(
    this IconToggleButton iconToggleButton,
    TSource source,
    string propertyName,
    Func<TSource, TProp> getter,
    Action<TSource, TProp> setter,
    BindingScope bindings)
    where TSource : class, INotifyPropertyChanged {

    EventHandler<CompoundButton.CheckedChangeEventArgs>? handler = null;

    bindings.Add(new ViewBinder<TSource, TProp, bool>(source, propertyName, getter, setter,
      v => { if (v != iconToggleButton.Checked) iconToggleButton.Checked = v; },
      eh => {
        handler = (s, e) => eh(s, e.IsChecked);
        iconToggleButton.CheckedChanged += handler;
      },
      eh => { if (handler != null) iconToggleButton.CheckedChanged -= handler; }));

    return iconToggleButton;
  }

  public static ProgressBar BindProgressBar<TSource>(this ProgressBar progressBar, TSource source, BindingScope bindings)
    where TSource : class, INotifyPropertyChanged, IProgressDialog {

    progressBar.Max = source.ProgressMax;
    source.Bind(nameof(IProgressDialog.ProgressValue), x => x.ProgressValue, x => progressBar.Progress = x).DisposeWith(bindings);

    return progressBar;
  }

  public static TextView BindProgressText<TSource>(this TextView progressText, TSource source, BindingScope bindings)
    where TSource : class, INotifyPropertyChanged, IProgressDialog {

    return progressText.BindText(source, nameof(IProgressDialog.ProgressText),
      x => x.ProgressText, x => x ?? string.Empty, bindings);
  }

  public static Slider BindProgress<TSource, TProp>(
    this Slider slider,
    TSource source,
    string propertyName,
    Func<TSource, TProp> getter,
    Action<TSource, TProp> setter,
    BindingScope bindings)
    where TSource : class, INotifyPropertyChanged {

    EventHandler<SeekBar.ProgressChangedEventArgs>? handler = null;

    bindings.Add(new ViewBinder<TSource, TProp, double>(source, propertyName, getter, setter,
      v => slider.Selector.Value = v,
      eh => {
        handler = (s, e) => {
          if (!e.FromUser) return;
          eh(s, ((Slider)s!).Selector.Value);
        };
        slider.ProgressChanged += handler;
      },
      eh => { if (handler != null) slider.ProgressChanged -= handler; }));

    return slider;
  }

  public static RadioGroup BindChecked<TSource, TEnum>(
    this RadioGroup radioGroup,
    TSource source,
    string propertyName,
    Func<TSource, TEnum> getter,
    Action<TSource, TEnum> setter,
    TEnum[] values,
    BindingScope bindings)
    where TSource : class, INotifyPropertyChanged
    where TEnum : struct, Enum {

    EventHandler<RadioGroup.CheckedChangeEventArgs>? handler = null;

    bindings.Add(new ViewBinder<TSource, TEnum, TEnum>(source, propertyName, getter, setter,
      v => {
        for (int i = 0; i < values.Length && i < radioGroup.ChildCount; i++) {
          if (EqualityComparer<TEnum>.Default.Equals(values[i], v)) {
            if (radioGroup.GetChildAt(i) is RadioButton rb && !rb.Checked) radioGroup.Check(rb.Id);
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
      }));

    return radioGroup;
  }

  public static Spinner BindSelected<TSource, TProp, TKey>(
    this Spinner spinner,
    TSource source,
    string propertyName,
    Func<TSource, TProp> getter,
    Action<TSource, TProp> setter,
    KeyValuePair<TKey, string>[] map,
    BindingScope bindings)
    where TSource : class, INotifyPropertyChanged {

    EventHandler<AdapterView.ItemSelectedEventArgs>? handler = null;

    var adapter = new ArrayAdapter<string>(spinner.Context!, Resource.Layout.spinner_item, [.. map.Select(x => x.Value)]);
    adapter.SetDropDownViewResource(Resource.Layout.spinner_dropdown_item);
    spinner.Adapter = adapter;

    bindings.Add(new ViewBinder<TSource, TProp, TKey>(source, propertyName, getter, setter,
      v => {
        var index = Array.FindIndex(map, kvp => Equals(kvp.Key, v));
        if (index >= 0 && spinner.SelectedItemPosition != index)
          spinner.SetSelection(index);
      },
      eh => {
        handler = (s, e) => eh(s, map[e.Position].Key);
        spinner.ItemSelected += handler;
      },
      eh => { if (handler != null) spinner.ItemSelected -= handler; }));

    return spinner;
  }

  public static TTarget BindVisibility<TTarget, TSource>(
    this TTarget view,
    TSource source,
    string propertyName,
    Func<TSource, bool> getter,
    BindingScope bindings)
    where TTarget : View
    where TSource : class, INotifyPropertyChanged {

    bindings.Add(source.Bind(propertyName, getter, p => view.Visibility = p ? ViewStates.Visible : ViewStates.Gone));
    return view;
  }
}