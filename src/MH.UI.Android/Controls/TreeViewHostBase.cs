using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Controls;
using MH.Utils.BaseClasses;
using MH.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MH.UI.Android.Controls;

public interface IAndroidTreeViewHost : ITreeViewHost {
  public TreeView DataContext { get; }
  public void ShowItemMenu(View anchor, object? item);
}

public abstract class TreeViewHostBase<TView, TAdapter> : RelativeLayout, IAndroidTreeViewHost
    where TView : TreeView
    where TAdapter : TreeViewHostAdapterBase {
  protected readonly RecyclerView _recyclerView;
  protected bool _disposed;
  private TreeView? _itemMenuVM;
  private TreeMenuHost? _itemMenuV;

  public TView DataContext { get; }
  public TAdapter? Adapter { get; set; }
  public Func<object, IEnumerable<MenuItem>>? ItemMenuFactory { get; }
  public event EventHandler<bool>? HostIsVisibleChangedEvent;
  TreeView IAndroidTreeViewHost.DataContext => DataContext;

  protected TreeViewHostBase(Context context, TView dataContext, Func<object, IEnumerable<MenuItem>>? itemMenuFactory) : base(context) {
    DataContext = dataContext;
    ItemMenuFactory = itemMenuFactory;
    SetBackgroundResource(Resource.Color.c_static_ba);

    _recyclerView = new(context) {
      LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
    };
    _recyclerView.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Vertical, false));
    AddView(_recyclerView);
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      HostIsVisibleChangedEvent = null;
      DataContext.Host = null;
      Adapter?.Dispose();
      _recyclerView.Dispose();
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  protected override void OnVisibilityChanged(View changedView, [GeneratedEnum] ViewStates visibility) {
    base.OnVisibilityChanged(changedView, visibility);
    HostIsVisibleChangedEvent?.Invoke(this, visibility == ViewStates.Visible);
  }

  public void ShowItemMenu(View anchor, object? item) {
    if (item == null) return;
    if (_itemMenuVM == null) {
      _itemMenuVM = new();
      _itemMenuV = new TreeMenuHost(Context!, _itemMenuVM);
    }

    if (ItemMenuFactory?.Invoke(item) is not { } menuItems) return;
    _itemMenuVM.RootHolder.Clear();
    foreach (var menuItem in menuItems)
      _itemMenuVM.RootHolder.Add(menuItem);

    if (_itemMenuV == null || _itemMenuVM.RootHolder.Count == 0) return;
    _itemMenuV.Observer.MenuAnchor = anchor;
    _itemMenuV.Adapter!.SetItemsSource();
    _itemMenuV.Popup.ShowAsDropDown(anchor);
  }

  public virtual void ExpandRootWhenReady(ITreeItem root) => root.IsExpanded = true;

  public virtual void ScrollToTop() { /* TODO PORT */ }

  public virtual void ScrollToItems(object[] items, bool exactly) {
    var item = items[^1];
    if (Adapter?.Items.SingleOrDefault(x => ReferenceEquals(x.TreeItem, item)) is not { } flatItem) return;
    var position = Adapter.Items.ToList().IndexOf(flatItem);

    if (exactly && _recyclerView.GetLayoutManager() is LinearLayoutManager layoutManager)
      layoutManager.ScrollToPositionWithOffset(position, 0);
    else
      _recyclerView.ScrollToPosition(position);
  }
}