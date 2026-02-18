using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Utils;

namespace MH.UI.Android.Controls.Recycler;

public class BaseViewHolder : RecyclerView.ViewHolder {
  public BaseViewHolder(View view, RecyclerView.LayoutParams viewLayoutParams) : base(view) {
    view.LayoutParameters = viewLayoutParams;
  }
}