using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Controls;
using MH.UI.Interfaces;
using MH.Utils.Interfaces;
using System;

namespace MH.UI.Android.Controls.Hosts.CollectionViewHost;

public class CollectionViewHost : TreeViewHostBase<CollectionView, CollectionViewHostAdapter>, ICollectionViewHost {
  public bool IsMultiSelectOn { get; set; }
  public Func<Context, ICollectionViewGroup, ICollectionViewItemContent> CreateItemContent { get; }
  public readonly RecyclerView.RecycledViewPool ItemViewPool = new RecyclerView.RecycledViewPool();

  public CollectionViewHost(
    Context context,
    CollectionView dataContext,
    Func<Context, ICollectionViewGroup, ICollectionViewItemContent> createItemContent) :
    base(context, dataContext, dataContext.GetMenu) {

    CreateItemContent = createItemContent;
    DataContext.Host = this;
    ((TreeView)DataContext).Host = this;
    Adapter = new CollectionViewHostAdapter(context, this);
    _recyclerView.SetAdapter(Adapter);
  }

  protected override void OnSizeChanged(int w, int h, int oldw, int oldh) {
    base.OnSizeChanged(w, h, oldw, oldh);

    if (Visibility == ViewStates.Visible && DataContext?.RootHolder is [ICollectionViewGroup { Width: 0 } group]) {
      group.Width = w;
      Adapter?.SetItemsSource();
    }
  }

  internal void HandleItemClick(ICollectionViewRow row, ISelectable? itemDataContext) {
    IsMultiSelectOn = false;
    if (itemDataContext == null) return;
    if (DataContext.CanSelect) DataContext.SelectItem(row, itemDataContext, false, false);
    if (DataContext.CanOpen) DataContext.OpenItem(itemDataContext);
  }

  internal void HandleItemLongClick(ICollectionViewRow row, ISelectable? itemDataContext) {
    if (itemDataContext == null) return;
    var isCtrlOn = IsMultiSelectOn;
    IsMultiSelectOn = true;
    if (DataContext.CanSelect) DataContext.SelectItem(row, itemDataContext, isCtrlOn, false);
  }
}