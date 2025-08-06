using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Controls;
using MH.UI.Interfaces;
using System;

namespace MH.UI.Android.Controls;

public class CollectionViewHost : BaseTreeViewHost<CollectionView, CollectionViewHostAdapter>, ICollectionViewHost {
  public bool IsMultiSelectOn { get; set; }
  public Func<LinearLayout, ICollectionViewGroup, object?, View?> GetItemView { get; }

  public CollectionViewHost(
    Context context,
    CollectionView dataContext,
    Func<View, object?, PopupWindow?> itemMenuFactory,
    Func<LinearLayout, ICollectionViewGroup, object?, View?> getItemView) :
    base(context, dataContext, itemMenuFactory) {

    GetItemView = getItemView;
    DataContext.Host = this;
    ((TreeView)DataContext).Host = this;
    _adapter = new CollectionViewHostAdapter(context, this);
    _recyclerView.SetAdapter(_adapter);
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) ((TreeView)DataContext).Host = null;
    base.Dispose(disposing);
  }

  protected override void OnSizeChanged(int w, int h, int oldw, int oldh) {
    base.OnSizeChanged(w, h, oldw, oldh);

    if (Visibility == ViewStates.Visible && DataContext?.RootHolder is [ICollectionViewGroup { Width: 0 } group]) {
      group.Width = w;
      _adapter?.SetItemsSource();
    }
  }

  internal void HandleItemClick(ICollectionViewRow row, CollectionViewItem? itemView) {
    IsMultiSelectOn = false;
    if (itemView == null) return;
    if (DataContext.CanSelect) DataContext.SelectItem(row, itemView.DataContext, false, false);
    if (DataContext.CanOpen) DataContext.OpenItem(itemView.DataContext);
  }

  internal void HandleItemLongClick(ICollectionViewRow row, CollectionViewItem? itemView) {
    if (itemView == null) return;
    var isCtrlOn = IsMultiSelectOn;
    IsMultiSelectOn = true;
    if (DataContext.CanSelect) DataContext.SelectItem(row, itemView.DataContext, isCtrlOn, false);
  }
}