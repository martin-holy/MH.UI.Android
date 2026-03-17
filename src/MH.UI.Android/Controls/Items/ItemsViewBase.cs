using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Controls.Recycler;
using MH.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MH.UI.Android.Controls.Items;

public abstract class ItemsViewBase<T> : RecyclerView {
  internal IReadOnlyList<T> _items;
  protected bool _disposed;

  public BindableAdapter<T>? ItemsAdapter { get; set; }

  public event Action<T>? ItemClickedEvent;

  protected ItemsViewBase(Context context, IReadOnlyList<T> items) : base(context) {
    _items = items;

    if (_items is ObservableCollection<T> newCol)
      newCol.CollectionChanged += _onCollectionChanged;
  }

  private void _raiseItemClicked(T item) =>
    ItemClickedEvent?.Invoke(item);

  internal virtual void HandleItemClick(T item) {
    _raiseItemClicked(item);
  }

  internal void _onViewCreated(View view) {
    view.Click += (o, _) => {
      if (o is IBindable<T> { DataContext: { } dc })
        HandleItemClick(dc);
    };
  }

  private void _onCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
    if (ItemsAdapter == null) return;
    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
      ItemsAdapter.NotifyItemRangeInserted(e.NewStartingIndex, e.NewItems.Count);
    else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
      ItemsAdapter.NotifyItemRangeRemoved(e.OldStartingIndex, e.OldItems.Count);
    else
      ItemsAdapter.NotifyDataSetChanged();
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      if (_items is ObservableCollection<T> oldCol)
        oldCol.CollectionChanged -= _onCollectionChanged;

      SetAdapter(null);
      ItemsAdapter?.Dispose();
    }
    _disposed = true;
    base.Dispose(disposing);
  }
}