using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls;

public class CollectionViewHostAdapter(Context context, CollectionViewHost _host) : TreeViewHostAdapter(context, _host.DataContext) {
  public override int GetItemViewType(int position) =>
    _items[position] is FlatTreeItem { TreeItem: ICollectionViewGroup } ? 0 : 1;

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
    viewType == 0
      ? CollectionViewGroupViewHolder.Create(parent)
      : new CollectionViewRowViewHolder(parent.Context!, _host);

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
    var item = _items[position];

    if (holder is CollectionViewGroupViewHolder groupHolder)
      groupHolder.Bind(item);
    else if (holder is CollectionViewRowViewHolder rowHolder)
      rowHolder.Bind(item);
  }
}