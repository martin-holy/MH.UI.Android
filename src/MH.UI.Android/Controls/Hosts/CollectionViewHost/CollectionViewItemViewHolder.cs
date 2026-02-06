using AndroidX.RecyclerView.Widget;
using MH.Utils.Interfaces;

namespace MH.UI.Android.Controls.Hosts.CollectionViewHost;

public class CollectionViewItemViewHolder : RecyclerView.ViewHolder {
  public CollectionViewItemViewHolder(CollectionViewItemShell shell) : base(shell) { }

  public void Bind(ISelectable dataContext, int itemWidth, int itemHeight) =>
    ((CollectionViewItemShell)ItemView).Bind(dataContext, itemWidth, itemHeight);

  public void Unbind() =>
    ((CollectionViewItemShell)ItemView).Unbind();
}