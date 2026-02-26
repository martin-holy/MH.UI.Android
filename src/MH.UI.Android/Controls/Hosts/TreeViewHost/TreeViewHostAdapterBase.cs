using Android.Content;
using AndroidX.RecyclerView.Widget;
using MH.Utils;
using MH.Utils.BaseClasses;
using MH.Utils.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MH.UI.Android.Controls.Hosts.TreeViewHost;

public abstract class TreeViewHostAdapterBase : RecyclerView.Adapter {
  protected readonly Context _context;
  internal FlatTree _flatTree;
  protected bool _disposed;
  protected static readonly Java.Lang.Object _isExpandedPayload = new Java.Lang.String("IsExpanded");
  protected static readonly Java.Lang.Object _isExpandedVisiblePayload = new Java.Lang.String("IsExpandedVisible");

  public override int ItemCount => _flatTree.Items.Count;
  public IReadOnlyList<FlatTreeItem> Items => _flatTree.Items;

  protected TreeViewHostAdapterBase(Context context, ObservableCollection<ITreeItem> rootHolder) {
    _context = context;
    _flatTree = new(rootHolder);
    _flatTree.ResetEvent += () => NotifyDataSetChanged();
    _flatTree.RangeInsertedEvent += (index, count) => NotifyItemRangeInserted(index, count);
    _flatTree.RangeRemovedEvent += (index, count) => NotifyItemRangeRemoved(index, count);
    _flatTree.IsExpandedChangedEvent += (index) => NotifyItemChanged(index, _isExpandedPayload);
    _flatTree.IsExpandedVisibilityChangedEvent += (index) => NotifyItemChanged(index, _isExpandedVisiblePayload);
    Reset();
  }

  internal void Reset() {
    if (_disposed) return;
    Tasks.Dispatch(() => _flatTree.Reset());
  }

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position, IList<Java.Lang.Object> payloads) {
    if (payloads.Count == 0) {
      OnBindViewHolder(holder, position);
      return;
    }

    if (holder.ItemView is not FlatTreeItemVBase view) return;
    var item = Items[position];

    foreach (var payload in payloads)
      if (ReferenceEquals(payload, _isExpandedPayload))
        view.BindIsExpanded(item);
      else if (ReferenceEquals(payload, _isExpandedVisiblePayload))
        view.BindIsExpandedVisible(item);
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing)
      _flatTree.Clear();
    _disposed = true;
    base.Dispose(disposing);
  }
}