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
  private bool _disposed;

  public override int ItemCount => _items.Length;
  public IReadOnlyList<FlatTreeItem> Items => _items;

  protected TreeViewHostAdapterBase(Context context, ObservableCollection<ITreeItem> rootHolder) {
    _context = context;
    _rootHolder = rootHolder;
    _rootHolder.CollectionChanged += _onRootHolderCollectionChanged;
    SetItemsSource();
  }

  internal void SetItemsSource() {
    var newFlatItems = Tree.ToFlatTreeItems(_rootHolder);
    _updateTreeItemSubscriptions(_items, newFlatItems);
    _items = [.. newFlatItems];
    Tasks.Dispatch(NotifyDataSetChanged);
  }

  private void _onTreeItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
    SetItemsSource();

  private void _updateTreeItemSubscriptions(IEnumerable<FlatTreeItem> oldItems, IEnumerable<FlatTreeItem> newItems) {
    var o = oldItems.Except(newItems).ToArray();
    var n = newItems.Except(oldItems).ToArray();

    foreach (var item in o) {
      item.TreeItem.PropertyChanged -= _onTreeItemPropertyChanged;
      item.TreeItem.Items.CollectionChanged -= _onTreeItemsChanged;
      if (item.TreeItem is ILeafyTreeItem { Leaves: INotifyCollectionChanged leaves })
        leaves.CollectionChanged -= _onTreeItemsChanged;
    }

    foreach (var item in n) {
      item.TreeItem.PropertyChanged += _onTreeItemPropertyChanged;
      item.TreeItem.Items.CollectionChanged += _onTreeItemsChanged;
      if (item.TreeItem is ILeafyTreeItem { Leaves: INotifyCollectionChanged leaves })
        leaves.CollectionChanged += _onTreeItemsChanged;
    }
  }

  protected void _onTreeItemPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    if (e.Is(nameof(TreeItem.IsExpanded)))
      SetItemsSource();
  }

  private void _onRootHolderCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
    SetItemsSource();

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _rootHolder.CollectionChanged -= _onRootHolderCollectionChanged;
    }
    _disposed = true;
    base.Dispose(disposing);
  }
}