﻿using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Controls;
using MH.Utils;
using MH.Utils.BaseClasses;
using MH.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MH.UI.Android.Controls;

public class TreeViewHostAdapter : RecyclerView.Adapter {
  protected readonly Context _context;
  protected readonly TreeView _viewModel;
  protected object[] _items = [];
  private readonly Handler _handler = new(Looper.MainLooper);

  public TreeViewHostAdapter(Context context, TreeView viewModel) {
    _context = context;
    _viewModel = viewModel;
    _viewModel.RootHolder.CollectionChanged += _onTreeItemsChanged;
    _setItemsSource();
  }

  public override int ItemCount => _items.Length;

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
    var view = LayoutInflater.From(_context)?.Inflate(Resource.Layout.flat_tree_item, parent, false);
    return new FlatTreeItemViewHolder(view, _viewModel);
  }

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) =>
    ((FlatTreeItemViewHolder)holder).Bind(_items[position] as FlatTreeItem);

  public void UpdateItems(IEnumerable? newItems) {
    _items = newItems == null ? [] : [.. newItems.Cast<object>()];
    _handler.Post(NotifyDataSetChanged);
  }

  private void _onTreeItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
    _setItemsSource();

  private void _setItemsSource() {
    var newFlatItems = Tree.ToFlatTreeItems(_viewModel.RootHolder);
    _updateTreeItemSubscriptions(_items as IEnumerable<FlatTreeItem>, newFlatItems);
    UpdateItems(newFlatItems);
  }

  private void _updateTreeItemSubscriptions(IEnumerable<FlatTreeItem>? oldItems, IEnumerable<FlatTreeItem>? newItems) {
    var o = oldItems?.Except(newItems ?? []).ToArray() ?? [];
    var n = newItems?.Except(oldItems ?? []).ToArray() ?? [];

    foreach (var item in o)
      item.TreeItem.PropertyChanged -= _onTreeItemPropertyChanged;

    foreach (var item in n)
      item.TreeItem.PropertyChanged += _onTreeItemPropertyChanged;
  }

  private void _onTreeItemPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    if (e.Is(nameof(TreeItem.IsExpanded)))
      _setItemsSource();
  }
}