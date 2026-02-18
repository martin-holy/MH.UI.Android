using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Controls.Recycler;
using MH.UI.Android.Utils;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls.Hosts.CollectionViewHost;

public class CollectionViewHostAdapter(Context _context, CollectionViewHost _host)
  : TreeViewHostAdapterBase(_context, _host.DataContext.RootHolder) {

  public override int GetItemViewType(int position) =>
    _items[position] is FlatTreeItem { TreeItem: ICollectionViewGroup } ? 0 : 1;

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
    viewType == 0
      ? new BaseViewHolder(new CollectionViewGroupV(parent.Context!, _host), new(LPU.Match, LPU.Wrap))
      : new BaseViewHolder(new CollectionViewRowV(parent.Context!, _host), new(LPU.Match, LPU.Wrap));

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) =>
    (holder.ItemView as IBindable<FlatTreeItem>)?.Bind(_items[position]);
}