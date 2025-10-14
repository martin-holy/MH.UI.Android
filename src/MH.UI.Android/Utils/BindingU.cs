using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.Utils.BaseClasses;
using MH.Utils.Binding;
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

  public static Slider BindProgress<TSource, TProp>(
    this Slider slider,
    TSource source,
    Expression<Func<TSource, TProp>> property,
    MH.Utils.BindingU.Mode mode = MH.Utils.BindingU.Mode.TwoWay)
    where TSource : class, INotifyPropertyChanged {

    EventHandler<global::Android.Widget.SeekBar.ProgressChangedEventArgs>? handler = null;

    new ViewBinder<Slider, double>(
      slider,
      subscribe: eh => {
        handler = (s, e) => eh(s, e.Progress / 10.0 + slider.MinD);
        slider.ProgressChanged += handler;
      },
      unsubscribe: eh => {
        if (handler != null) slider.ProgressChanged -= handler;
      },
      getViewValue: v => v.Progress / 10.0 + v.MinD,
      setViewValue: (v, val) => {
        int newProgress = (int)((val - v.MinD) * 10);
        if (v.Progress != newProgress)
          v.Progress = newProgress;
      }).Bind(source, property, mode);

    return slider;
  }
}