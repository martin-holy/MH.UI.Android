using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Controls.Hosts.TreeViewHost;
using MH.UI.Android.Controls.Recycler;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;
using MH.Utils.Extensions;
using MH.Utils.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace MH.UI.Android.Controls.Hosts.CollectionViewHost;

public class CollectionViewHostAdapter : TreeViewHostAdapterBase {

  private static readonly Java.Lang.Object _rowItemsPayload = new Java.Lang.String("RowItems");
  private static readonly Java.Lang.Object _sourceCountPayload = new Java.Lang.String("SourceCount");
  private readonly CollectionViewHost host;

  public CollectionViewHostAdapter(Context _context, CollectionViewHost _host) : base(_context, _host.DataContext.RootHolder) {
    host = _host;
    _flatTree.TreeItemPropertyChangedEvent += _onTreeItemPropertyChanged;
  }

  public override int GetItemViewType(int position) =>
    Items[position] is FlatTreeItem { TreeItem: ICollectionViewGroup } ? 0 : 1;

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
    viewType == 0
      ? new BaseViewHolder(
          new CollectionViewGroupV(parent.Context!, host),
          new RecyclerView.LayoutParams(LPU.Match, LPU.Wrap).WithDpMargin(2, 1, 2, 1))
      : new BaseViewHolder(new CollectionViewRowV(parent.Context!, host), new(LPU.Match, LPU.Wrap));

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) =>
    (holder.ItemView as IBindable<FlatTreeItem>)?.Bind(Items[position]);

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position, IList<Java.Lang.Object> payloads) {
    base.OnBindViewHolder(holder, position, payloads);
    if (payloads.Count == 0) return;

    var item = Items[position];

    foreach (var payload in payloads) {
      if (ReferenceEquals(payload, _rowItemsPayload))
        (holder.ItemView as CollectionViewRowV)?.BindItems(item);
      else if (ReferenceEquals(payload, _sourceCountPayload))
        (holder.ItemView as CollectionViewGroupV)?.BindSourceCount((ICollectionViewGroup)item.TreeItem);
    }
  }

  private void _onTreeItemPropertyChanged(ITreeItem item, int index, PropertyChangedEventArgs e) {
    if (e.Is(nameof(ICollectionViewGroup.SourceCount)))
      NotifyItemChanged(index, _sourceCountPayload);
    else if (e.Is(nameof(ICollectionViewRow.Hash)))
      NotifyItemChanged(index, _rowItemsPayload);
  }
}