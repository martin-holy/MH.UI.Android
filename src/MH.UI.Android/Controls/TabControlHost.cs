using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils;
using MH.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace MH.UI.Android.Controls;

public class TabControlHost : LinearLayout {
  private readonly LinearLayout _headerPanel;
  private readonly RecyclerView _tabHeaders;
  private readonly FrameLayout _tabContent;
  private readonly TabControlHostHeaderAdapter _adapter;
  private readonly Dictionary<IListItem, View> _contentViews = [];
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

    if (dataContext.TabStrip.Placement is Dock.Top or Dock.Bottom)
      _createVertical();
    else
      _createHorizontal();

    if (slotFactory?.Invoke(dataContext.TabStrip.Slot) is View slot) {
      _headerPanel.AddView(
        slot,
        dataContext.TabStrip.SlotPlacement is Dock.Left or Dock.Top ? 0 : -1,
        new LinearLayout.LayoutParams(LPU.Wrap, LPU.Wrap));
    }

    this.Bind(DataContext, nameof(TabControl.Tabs), x => x.Tabs, (_, _, e) => _onTabsChanged(null, e));
    this.Bind(DataContext, nameof(TabControl.Selected), x => x.Selected, (_, _) => _updateContent());
    _updateContent();
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

  protected virtual View? _getItemView(LinearLayout container, object? item) => null;

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
    if (e.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Reset)
      foreach (var item in e.OldItems?.Cast<IListItem>() ?? [])
        if (_contentViews.Remove(item, out var view)) {
          _tabContent.RemoveView(view);
          view.Dispose();
        }

    _updateContent();
  }

  private void _updateContent() {
    if (DataContext.Selected is not { } selectedItem) {
      _adapter.NotifyDataSetChanged();
      return;
    }

    if (!_contentViews.TryGetValue(selectedItem, out var view)) {
      var container = new LinearLayout(Context!);
      view = _getItemView(container, selectedItem.Data);
      if (view != null) {
        container.AddView(view, new LayoutParams(LPU.Match, LPU.Match));
        _contentViews[selectedItem] = container;
      }
    }

    if (view != null)
      foreach (var kvp in _contentViews) {
        kvp.Value.Visibility = kvp.Key == selectedItem ? ViewStates.Visible : ViewStates.Invisible;
        if (kvp.Value.Parent == null)
          _tabContent.AddView(kvp.Value, new LayoutParams(LPU.Match, LPU.Match));
      }

    _adapter.NotifyDataSetChanged();
  }
}