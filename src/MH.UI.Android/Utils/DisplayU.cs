using Android.Util;

namespace MH.UI.Android.Utils;

public class DisplayU {
  public static DisplayMetrics Metrics { get; private set; }

  public static void Init(DisplayMetrics displayMetrics) {
    Metrics = displayMetrics;
  }

  public static int DpToPx(float dp) => (int)(dp * Metrics.Density);
  public static float PxToDp(int px) => px / Metrics.Density;
}