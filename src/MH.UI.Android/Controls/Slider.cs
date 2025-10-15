using Android.Content;
using Android.Widget;
using MH.Utils;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MH.UI.Android.Controls;

public class Slider : SeekBar {
  public double MinD { get; }
  public double TickFrequency { get; }

  private readonly double _scale;
  private ViewBinder<Slider, double>? _progressBinder;
  private EventHandler<ProgressChangedEventArgs>? _progressHandler;

  public Slider(Context context, double minD, double maxD, double tickFrequency) : base(context) {
    MinD = minD;
    TickFrequency = tickFrequency <= 0 ? (maxD - minD) / 100 : tickFrequency;
    _scale = 1.0 / TickFrequency;
    Max = (int)((maxD - minD) * _scale);
  }

  public Slider BindProgress<TSource, TProp>(TSource source, Expression<Func<TSource, TProp>> property)
    where TSource : class, INotifyPropertyChanged {

    if (_progressBinder != null && _progressHandler != null)
      ProgressChanged -= _progressHandler;

    _progressBinder = new ViewBinder<Slider, double>(
      this,
      eh => {
        _progressHandler = (s, e) => {
          var sl = (Slider)s!;
          if (!e.FromUser) return;

          double value = e.Progress / sl._scale + sl.MinD;
          double snapped = Math.Round(value / sl.TickFrequency) * sl.TickFrequency;

          int snappedProgress = (int)Math.Round((snapped - sl.MinD) * sl._scale);
          if (snappedProgress != e.Progress) {
            sl.Progress = snappedProgress;
            eh(s, snapped);
          }
          else {
            eh(s, snapped);
          }
        };
        ProgressChanged += _progressHandler;
      },
      eh => {
        if (_progressHandler != null) {
          ProgressChanged -= _progressHandler;
          _progressHandler = null;
        }
      },
      (v, val) => v.Progress = (int)Math.Round((val - v.MinD) * v._scale)
    );

    _progressBinder.Bind(source, property);
    return this;
  }

  protected override void Dispose(bool disposing) {
    if (disposing && _progressHandler != null)
      ProgressChanged -= _progressHandler;
    _progressBinder = null;
    _progressHandler = null;
    base.Dispose(disposing);
  }
}