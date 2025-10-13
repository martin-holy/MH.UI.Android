using Android.Content;
using Android.Widget;
using MH.Utils;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MH.UI.Android.Controls;

public class Slider : SeekBar {
  private readonly double _minD;
  private bool _disposed;
  private Action? _disposeBinding;

  public Slider(Context context, double minD, double maxD) : base(context) {
    _minD = minD;
    Max = (int)((maxD - minD) * 10);
  }

  public Slider BindProgress<TSource, TProp>(
    TSource source,
    Expression<Func<TSource, TProp>> propertyExpression,
    BindingU.Mode mode = BindingU.Mode.TwoWay)
    where TSource : class, INotifyPropertyChanged {

    if (propertyExpression.Body is not MemberExpression m)
      throw new ArgumentException("Expression must be a property access", nameof(propertyExpression));

    var propertyName = m.Member.Name;
    var getter = BindingU.GetterCache.GetGetter<TSource, TProp>(propertyName);
    var setter = BindingU.SetterCache.GetSetter<TSource, TProp>(propertyName);

    IDisposable? sub = null;
    EventHandler<ProgressChangedEventArgs>? handler = null;
    bool updating = false;

    // --- ViewModel → Slider (OneWay)
    sub = BindingU.Bind(this, source, propertyExpression, (v, p) => {
      updating = true;
      try {
        double value = Convert.ToDouble(p);
        var newProgress = (int)((value - v._minD) * 10);
        if (v.Progress != newProgress)
          v.Progress = newProgress;
      }
      finally { updating = false; }
    });

    // --- Slider → ViewModel (TwoWay)
    if (mode == BindingU.Mode.TwoWay) {
      handler = (_, e) => {
        if (updating) return;
        var newVal = e.Progress / 10.0 + _minD;

        object converted = typeof(TProp) == typeof(int)
          ? (object)(int)Math.Round(newVal)
          : (object)newVal;

        setter(source, (TProp)converted);
      };
      ProgressChanged += handler;
    }

    var oldDispose = _disposeBinding;
    _disposeBinding = () => {
      sub?.Dispose();
      if (handler != null)
        ProgressChanged -= handler;
      oldDispose?.Invoke();
    };

    return this;
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _disposeBinding?.Invoke();
      _disposeBinding = null;
    }
    _disposed = true;
    base.Dispose(disposing);
  }
}