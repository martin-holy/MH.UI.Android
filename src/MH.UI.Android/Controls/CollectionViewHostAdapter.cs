using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Interfaces;
using MH.Utils;
using MH.Utils.BaseClasses;
using MH.Utils.Extensions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MH.UI.Android.Controls;

public class CollectionViewHostAdapter : RecyclerView.Adapter {
  protected readonly Context _context;
  protected readonly CollectionViewHost _host;
  protected FlatTreeItem[] _items = [];

  public CollectionViewHostAdapter(Context context, CollectionViewHost host) {
    _context = context;
    _host = host;
    _host.DataContext.RootHolder.CollectionChanged += _onTreeItemsChanged;
    SetItemsSource();
  }

  public override int ItemCount => _items.Length;

  public override int GetItemViewType(int position) =>
    _items[position] is FlatTreeItem { TreeItem: ICollectionViewGroup } ? 0 : 1;

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
    viewType == 0
      ? new CollectionViewGroupViewHolder(parent.Context!)
      : new CollectionViewRowViewHolder(parent.Context!, _host);

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
    var item = _items[position];

    if (holder is CollectionViewGroupViewHolder groupHolder)
      groupHolder.Bind(item);
    else if (holder is CollectionViewRowViewHolder rowHolder)
      rowHolder.Bind(item);
  }

  private void _onTreeItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
    SetItemsSource();

  internal void SetItemsSource() {
    var newFlatItems = Tree.ToFlatTreeItems(_host.DataContext.RootHolder);
    _updateTreeItemSubscriptions(_items, newFlatItems);
    _items = [.. newFlatItems];
    Tasks.Dispatch(NotifyDataSetChanged);
  }

  private void _updateTreeItemSubscriptions(IEnumerable<FlatTreeItem> oldItems, IEnumerable<FlatTreeItem> newItems) {
    var o = oldItems.Except(newItems).ToArray();
    var n = newItems.Except(oldItems).ToArray();

    foreach (var item in o)
      item.TreeItem.PropertyChanged -= _onTreeItemPropertyChanged;

    foreach (var item in n)
      item.TreeItem.PropertyChanged += _onTreeItemPropertyChanged;
  }

  private void _onTreeItemPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    if (e.Is(nameof(TreeItem.IsExpanded)))
      SetItemsSource();
  }
}