using Android.Content;
using AndroidX.RecyclerView.Widget;
using MH.Utils;
using MH.Utils.BaseClasses;
using MH.Utils.Extensions;
using MH.Utils.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MH.UI.Android.Controls;

public abstract class TreeViewHostAdapterBase : RecyclerView.Adapter {
  protected readonly Context _context;
  protected readonly ObservableCollection<ITreeItem> _rootHolder;
  protected FlatTreeItem[] _items = [];
  protected bool _disposed;

  public override int ItemCount => _items.Length;
  public IReadOnlyList<FlatTreeItem> Items => _items;

  protected TreeViewHostAdapterBase(Context context, ObservableCollection<ITreeItem> rootHolder) {
    _context = context;
    _rootHolder = rootHolder;
    this.Bind(_rootHolder, (o, _, _) => o.SetItemsSource());
    SetItemsSource();
  }

  internal void SetItemsSource() {
    if (_disposed) return;
    var newFlatItems = Tree.ToFlatTreeItems(_rootHolder);
    var oldItems = _items.Except(newFlatItems).ToArray();
    var newItems = newFlatItems.Except(_items).ToArray();
    _unsubscribe(oldItems);
    _subscribe(newItems);
    _items = [.. newFlatItems];
    Tasks.Dispatch(NotifyDataSetChanged);
  }

  private void _subscribe(FlatTreeItem[] items) {
    foreach (var item in items) {
      item.TreeItem.PropertyChanged += _onTreeItemPropertyChanged;
      item.TreeItem.Items.CollectionChanged += _onTreeItemsChanged;
      if (item.TreeItem is ILeafyTreeItem { Leaves: INotifyCollectionChanged leaves })
        leaves.CollectionChanged += _onTreeItemsChanged;
    }
  }

  private void _unsubscribe(FlatTreeItem[] items) {
    foreach (var item in items) {
      item.TreeItem.PropertyChanged -= _onTreeItemPropertyChanged;
      item.TreeItem.Items.CollectionChanged -= _onTreeItemsChanged;
      if (item.TreeItem is ILeafyTreeItem { Leaves: INotifyCollectionChanged leaves })
        leaves.CollectionChanged -= _onTreeItemsChanged;
    }
  }

  protected virtual void _onTreeItemPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    if (e.Is(nameof(TreeItem.IsExpanded)))
      SetItemsSource();
  }

  private void _onTreeItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
    SetItemsSource();

  protected int _findIndexOfTreeItem(ITreeItem treeItem) {
    for (var i = 0; i < _items.Length; i++)
      if (ReferenceEquals(_items[i].TreeItem, treeItem))
        return i;

    return -1;
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _unsubscribe(_items);
      _items = [];
    }
    _disposed = true;
    base.Dispose(disposing);
  }
}