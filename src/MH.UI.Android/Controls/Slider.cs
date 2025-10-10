using Android.Content;
using Android.Widget;
using System;

namespace MH.UI.Android.Controls;

// TODO TwoWay binding for progress
public class Slider : SeekBar {
  private readonly double _minD;
  private readonly Action<double> _onChanged;
  private bool _disposed;

  public Slider(Context context, double minD, double maxD, double progressD, Action<double> onChanged) : base(context) {
    _minD = minD;
    _onChanged = onChanged;
    Max = (int)((maxD - minD) * 10);
    Progress = (int)((progressD - minD) * 10);
    ProgressChanged += _onProgressChanged;
  }

  private void _onProgressChanged(object? sender, ProgressChangedEventArgs e) {
    _onChanged(e.Progress / 10.0 + _minD);
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      ProgressChanged -= _onProgressChanged;
    }
    _disposed = true;
    base.Dispose(disposing);
  }
}
