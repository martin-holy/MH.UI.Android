using Android.Content;
using Android.Widget;

namespace MH.UI.Android.Controls;

public class Slider : SeekBar {
  public double MinD { get; }
  public double Scale { get; }
  public double TickFrequency { get; }  

  public Slider(Context context, double minD, double maxD, double tickFrequency) : base(context) {
    MinD = minD;
    TickFrequency = tickFrequency <= 0 ? (maxD - minD) / 100 : tickFrequency;
    Scale = 1.0 / TickFrequency;
    Max = (int)((maxD - minD) * Scale);
  }
}