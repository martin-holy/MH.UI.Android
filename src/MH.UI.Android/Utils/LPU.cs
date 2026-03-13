using Android.Views;
using Android.Widget;

namespace MH.UI.Android.Utils;

public static class LPU {
  public const int Match = ViewGroup.LayoutParams.MatchParent;
  public const int Wrap = ViewGroup.LayoutParams.WrapContent;

  public static LinearLayout.LayoutParams Linear(int w, int h) =>
    new(w, h);

  public static LinearLayout.LayoutParams Linear(int w, int h, float weight) =>
    new(w, h, weight);

  public static LinearLayout.LayoutParams Linear(int w, int h, GravityFlags gravity) =>
    new(w, h) { Gravity = gravity };

  public static LinearLayout.LayoutParams Linear(int w, int h, float weight, GravityFlags gravity) =>
    new(w, h, weight) { Gravity = gravity };
}