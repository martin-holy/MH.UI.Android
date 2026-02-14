using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Utils;
using MH.UI.Interfaces;

namespace MH.UI.Android.Controls.Items;

public class ItemViewHolder<T> : RecyclerView.ViewHolder {
  private T? _dataContext;

  public ItemViewHolder(View itemView, ItemsViewBase<T> owner) : base(itemView) {
    itemView.LayoutParameters = new RecyclerView.LayoutParams(LPU.Match, LPU.Wrap);
    itemView.Click += (_, _) => {
      if (_dataContext != null)
        owner.HandleItemClick(_dataContext);
    };
  }

  public void Bind(T item) {
    _dataContext = item;
    (ItemView as IBindable<T>)?.Bind(item);
  }
}