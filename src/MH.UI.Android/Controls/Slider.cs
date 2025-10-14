using Android.Content;
using Android.Widget;

namespace MH.UI.Android.Controls;

public class Slider : SeekBar {
  public double MinD { get; }

  public Slider(Context context, double minD, double maxD) : base(context) {
    MinD = minD;
    Max = (int)((maxD - minD) * 10);
  }
}