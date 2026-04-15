using Android.Content;
using Android.Widget;
using MH.UI.Controls;
using System;
using System.ComponentModel;

namespace MH.UI.Android.Controls;

public class Slider : SeekBar {
  private bool _updatingFromModel;

  public ValueSelector Selector { get; }

  public Slider(Context? context) : this(context, new ValueSelector()) { }

  public Slider(Context? context, double minimum, double maximum, double tickFrequency)
    : this(context, new() { Minimum = minimum, Maximum = maximum, TickFrequency = tickFrequency }) { }

  public Slider(Context? context, ValueSelector selector) : base(context) {
    Selector = selector;

    Selector.PropertyChanged += _onSelectorPropertyChanged;
    ProgressChanged += _onProgressChanged;

    _refreshRange();
    _refreshValue();
  }

  private void _onSelectorPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    switch (e.PropertyName) {
      case nameof(ValueSelector.Minimum):
      case nameof(ValueSelector.Maximum):
      case nameof(ValueSelector.TickFrequency):
        _refreshRange();
        _refreshValue();
        break;

      case nameof(ValueSelector.Value):
        _refreshValue();
        break;
    }
  }

  private void _onProgressChanged(object? sender, ProgressChangedEventArgs e) {
    if (_updatingFromModel || !e.FromUser) return;
    Selector.Value = _fromProgress(e.Progress);
  }

  private void _refreshRange() {
    Max = _toProgress(Selector.Maximum);
  }

  private void _refreshValue() {
    var progress = _toProgress(Selector.Value);

    if (Progress == progress) return;

    _updatingFromModel = true;
    Progress = progress;
    _updatingFromModel = false;
  }

  private int _toProgress(double value) =>
    (int)Math.Round((value - Selector.Minimum) / Selector.TickFrequency);

  private double _fromProgress(int progress) =>
    Selector.Minimum + progress * Selector.TickFrequency;

  protected override void Dispose(bool disposing) {
    if (disposing) {
      Selector.PropertyChanged -= _onSelectorPropertyChanged;
      ProgressChanged -= _onProgressChanged;
    }

    base.Dispose(disposing);
  }
}