using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Controls.Hosts.TreeMenuHost;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils;
using MH.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MH.UI.Android.Controls.Hosts.TabControlHost;

public class TabControlHost : LinearLayout {
  private readonly LinearLayout _headerPanel;
  private readonly RecyclerView _tabHeaders;
  private readonly FrameLayout _tabContent;
  private readonly TabControlHostHeaderAdapter _adapter;
  private readonly Dictionary<IListItem, View> _contentViews = new(ReferenceEqualityComparer.Instance);
  private IListItem? _previousSelected;
  private bool _disposed;

  public TabControl DataContext { get; }
  public TreeMenu ItemMenu { get; }

  public TabControlHost(Context context, TabControl dataContext, Func<object?, View?>? slotFactory = null) : base(context) {
    DataContext = dataContext;
    ItemMenu = new TreeMenu(context, dataContext.ItemMenuFactory);
    SetBackgroundResource(Resource.Color.c_static_ba);

    _headerPanel = new LinearLayout(context);
    _tabContent = new FrameLayout(context);
    _adapter = new TabControlHostHeaderAdapter(this);

    _tabHeaders = new RecyclerView(context) { ScrollBarStyle = ScrollbarStyles.OutsideOverlay };
    _tabHeaders.SetPadding(DimensU.Spacing);
    _tabHeaders.SetAdapter(_adapter);
    _tabHeaders.SetItemAnimator(null);

    if (dataContext.TabStrip.Placement is Dock.Top or Dock.Bottom)
      _createVertical();
    else
      _createHorizontal();

    if (slotFactory?.Invoke(dataContext.TabStrip.Slot) is View slot)
      _headerPanel.AddView(
        slot,
        dataContext.TabStrip.SlotPlacement is Dock.Left or Dock.Top ? 0 : -1,
        new LinearLayout.LayoutParams(LPU.Wrap, LPU.Wrap));

    this.Bind(DataContext, nameof(TabControl.Tabs), x => x.Tabs, (_, _, e) => _onTabsChanged(null, e), false);
    this.Bind(DataContext, nameof(TabControl.Selected), x => x.Selected, (_, _) => _onSelectedChanged());
  }

  private void _createHorizontal() {
    Orientation = Orientation.Horizontal;
    _tabHeaders.SetLayoutManager(new LinearLayoutManager(Context, LinearLayoutManager.Vertical, false));
    _headerPanel.Orientation = Orientation.Vertical;
    _headerPanel.AddView(_tabHeaders, new LinearLayout.LayoutParams(LPU.Wrap, 0, 1f));
    AddView(_tabContent, new LayoutParams(0, LPU.Match, 1));
    AddView(_headerPanel, _getViewIndex(DataContext.TabStrip.Placement), new LayoutParams(LPU.Wrap, LPU.Match));
  }

  private void _createVertical() {
    Orientation = Orientation.Vertical;
    _tabHeaders.SetLayoutManager(new LinearLayoutManager(Context, LinearLayoutManager.Horizontal, false));
    _headerPanel.Orientation = Orientation.Horizontal;
    _headerPanel.AddView(_tabHeaders, new LinearLayout.LayoutParams(0, LPU.Wrap, 1f));
    AddView(_tabContent, new LayoutParams(LPU.Match, 0, 1));
    AddView(_headerPanel, _getViewIndex(DataContext.TabStrip.Placement), new LayoutParams(LPU.Match, LPU.Wrap));
  }

  private int _getViewIndex(Dock dock) =>
    dock is Dock.Left or Dock.Top ? 0 : -1;

  protected virtual View? _getItemView(Context context, object? item) => null;

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      foreach (var view in _contentViews.Values) {
        _tabContent.RemoveView(view);
        view.Dispose();
      }
      _contentViews.Clear();

      _tabHeaders.SetAdapter(null);
      _adapter.Dispose();
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  private void _onTabsChanged(object? sender, NotifyCollectionChangedEventArgs e) {
    if (e.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Reset && e.OldItems != null)
      foreach (IListItem item in e.OldItems)
        if (_contentViews.Remove(item, out var view)) {
          _tabContent.RemoveView(view);
          view.Dispose();
        }

    _adapter.NotifyDataSetChanged();
  }

  private void _onSelectedChanged() {
    var current = DataContext.Selected;
    if (ReferenceEquals(_previousSelected, current)) return;

    if (_previousSelected != null && _contentViews.TryGetValue(_previousSelected, out var prevView))
      prevView.Visibility = ViewStates.Invisible;

    if (current != null) {
      if (!_contentViews.TryGetValue(current, out var view)) {
        view = _getItemView(Context!, current.Data);
        if (view != null) {
          _contentViews[current] = view;
          _tabContent.AddView(view, new FrameLayout.LayoutParams(LPU.Match, LPU.Match));
        }
      }

      if (view != null)
        view.Visibility = ViewStates.Visible;
    }

    _adapter.NotifyDataSetChanged();
    _previousSelected = current;
  }
}