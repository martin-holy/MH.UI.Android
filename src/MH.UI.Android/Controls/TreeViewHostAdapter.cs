using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.Utils.Extensions;
using MH.Utils.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace MH.UI.Android.Controls;

public class TreeViewHostAdapter(Context _context, TreeViewHost _host)
  : TreeViewHostAdapterBase(_context, _host.DataContext.RootHolder) {

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
    new FlatTreeItemViewHolder(parent.Context!, _host);

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) =>
    ((FlatTreeItemViewHolder)holder).Bind(_items[position]);

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position, IList<Java.Lang.Object> payloads) {
    if (payloads.Count == 0) {
      base.OnBindViewHolder(holder, position, payloads);
      return;
    }

    var item = _items[position];
    var vh = (FlatTreeItemViewHolder)holder;
    foreach (var payload in payloads) {
      if (Equals(nameof(ITreeItem.IsSelected), payload.ToString()))
        vh.UpdateSelection(item.TreeItem.IsSelected);
    }
  }

  protected override void _onTreeItemPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    base._onTreeItemPropertyChanged(sender, e);

    if (sender is not ITreeItem treeItem) return;
    var index = _findIndexOfTreeItem(treeItem);
    if (index == -1) return;

    if (_host.DataContext.ShowTreeItemSelection && e.Is(nameof(ISelectable.IsSelected)))
      NotifyItemChanged(index, nameof(ITreeItem.IsSelected));
  }
}