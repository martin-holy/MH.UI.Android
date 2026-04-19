using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Binding;
using MH.UI.Android.Controls.Hosts.TreeMenuHost;
using MH.UI.Android.Controls.Recycler;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils;
using MH.Utils.Disposables;
using MH.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MH.UI.Android.Controls.Hosts.TabControlHost;

public class TabControlHost : FrameLayout {
  private readonly LinearLayout _container;
  private readonly TextView _noTabsText;
  private readonly LinearLayout _headerPanel;
  private readonly RecyclerView _tabHeaders;
  private readonly FrameLayout _tabContent;
  private readonly BindableAdapter<IListItem> _adapter;
  private readonly Dictionary<IListItem, View> _contentViews = new(ReferenceEqualityComparer.Instance);
  private readonly BindingScope _bindings = new();
  private IListItem? _previousSelected;
  private bool _disposed;

  public TabControl DataContext { get; }
  public TreeMenu ItemMenu { get; }

  public TabControlHost(Context context, TabControl dataContext, Func<object?, View?>? slotFactory = null) : base(context) {
    DataContext = dataContext;
    ItemMenu = new TreeMenu(context, dataContext.ItemMenuFactory);
    SetBackgroundResource(Resource.Color.c_static_ba);

    _noTabsText = new TextView(context) { TextSize = 26 }
      .WithTextColor(Resource.Color.gray3)
      .BindText(dataContext, nameof(TabControl.NoTabsText), x => x.NoTabsText, x => x, _bindings)
      .BindVisibility(dataContext.Tabs, "Count", x => x.Count == 0, _bindings);
    _noTabsText.SetTypeface(null, global::Android.Graphics.TypefaceStyle.Bold);

    _headerPanel = new LinearLayout(context);
    _tabContent = new FrameLayout(context);
    _adapter = new(() => DataContext.Tabs, ctx => new TabItemHeaderV(ctx, this), () => new(LPU.Wrap, LPU.Wrap));

    _tabHeaders = new RecyclerView(context) { ScrollBarStyle = ScrollbarStyles.OutsideOverlay };
    _tabHeaders.SetPadding(DimensU.Spacing);
    _tabHeaders.SetAdapter(_adapter);
    _tabHeaders.SetItemAnimator(null);

    _container = dataContext.TabStrip.Placement is Dock.Top or Dock.Bottom
      ? _createVertical()
      : _createHorizontal();

    if (slotFactory?.Invoke(dataContext.TabStrip.Slot) is View slot)
      _headerPanel.AddView(
        slot,
        dataContext.TabStrip.SlotPlacement is Dock.Left or Dock.Top ? 0 : -1,
        LPU.LinearWrap());

    AddView(_noTabsText, LPU.Frame(LPU.Wrap, LPU.Wrap, GravityFlags.Center));
    AddView(_container, LPU.FrameMatch());

    DataContext.Bind(nameof(TabControl.Tabs), x => x.Tabs, _onTabsChanged, false).DisposeWith(_bindings);
    DataContext.Bind(nameof(TabControl.Selected), x => x.Selected, _ => _onSelectedChanged()).DisposeWith(_bindings);
  }

  private LinearLayout _createHorizontal() {
    _tabHeaders.SetLayoutManager(new LinearLayoutManager(Context, LinearLayoutManager.Vertical, false));
    _headerPanel.Orientation = Orientation.Vertical;
    _headerPanel.AddView(_tabHeaders, LPU.Linear(LPU.Wrap, 0, 1f));

    var layout = LayoutU.Horizontal(Context);
    layout.AddView(_tabContent, LPU.Linear(0, LPU.Match, 1));
    layout.AddView(_headerPanel, _getViewIndex(DataContext.TabStrip.Placement), LPU.Linear(LPU.Wrap, LPU.Match));
    return layout;
  }

  private LinearLayout _createVertical() {
    _tabHeaders.SetLayoutManager(new LinearLayoutManager(Context, LinearLayoutManager.Horizontal, false));
    _headerPanel.Orientation = Orientation.Horizontal;
    _headerPanel.AddView(_tabHeaders, LPU.Linear(0, LPU.Wrap, 1f));

    var layout = LayoutU.Vertical(Context);
    layout.AddView(_tabContent, LPU.Linear(LPU.Match, 0, 1));
    layout.AddView(_headerPanel, _getViewIndex(DataContext.TabStrip.Placement), LPU.LinearMatchWrap());
    return layout;
  }

  private int _getViewIndex(Dock dock) =>
    dock is Dock.Left or Dock.Top ? 0 : -1;

  protected virtual View? _getItemView(Context context, object? item) => null;

  public View? GetTabView(IListItem? tab) =>
    tab == null
      ? null
      : _contentViews.TryGetValue(tab, out var view)
        ? view
        : null;

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
      _bindings.Dispose();
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  private void _onTabsChanged(ObservableCollection<IListItem>? sender, NotifyCollectionChangedEventArgs e) {
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
          _tabContent.AddView(view, LPU.FrameMatch());
        }
      }

      if (view != null)
        view.Visibility = ViewStates.Visible;
    }

    _previousSelected = current;
  }
}