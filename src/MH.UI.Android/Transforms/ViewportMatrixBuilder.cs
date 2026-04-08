using Android.Graphics;
using MH.UI.Primitives;

namespace MH.UI.Android.Transforms;

public static class ViewportMatrixBuilder {
  public static Matrix BuildForBitmap(ViewportState state, double sourceW, double sourceH) {
    var matrix = new Matrix();

    if (!state.HasArea || sourceW <= 0 || sourceH <= 0) return matrix;

    // normalize source → content space
    var sx = (float)(state.ContentWidth / sourceW);
    var sy = (float)(state.ContentHeight / sourceH);
    matrix.SetScale(sx, sy);

    // apply viewport (content → host)
    matrix.PostScale(state.ScaleX, state.ScaleY);
    matrix.PostTranslate(state.TranslateX, state.TranslateY);

    return matrix;
  }
}