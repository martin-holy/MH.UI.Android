using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Controls;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls;

// TODO viewModel prop
public class CollectionViewHostAdapter(Context context, CollectionView viewModel) : TreeViewHostAdapter(context, viewModel) {
  public override int GetItemViewType(int position) =>
    _items[position] is FlatTreeItem { TreeItem: ICollectionViewGroup } ? 0 : 1;

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
    var inflater = LayoutInflater.From(_context);

    if (viewType == 0)
      return new CollectionViewGroupViewHolder(inflater.Inflate(Resource.Layout.collection_view_group, parent, false));
    else
      return new CollectionViewRowViewHolder(inflater.Inflate(Resource.Layout.collection_view_row, parent, false));
  }

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
    var item = _items[position] as FlatTreeItem;

    if (holder is CollectionViewGroupViewHolder groupHolder)
      groupHolder.Bind(item);
    else if (holder is CollectionViewRowViewHolder rowHolder)
      rowHolder.Bind(item);
  }
}