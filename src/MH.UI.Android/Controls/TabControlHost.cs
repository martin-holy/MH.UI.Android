using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Controls;
using MH.Utils.Extensions;
using MH.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace MH.UI.Android.Controls;

public class TabControlHost : LinearLayout {
  private readonly TabControl _dataContext;
  private readonly RecyclerView _tabHeaders;
  private readonly FrameLayout _tabContent;
  private readonly TabControlHostHeaderAdapter _adapter;
  private readonly Dictionary<IListItem, View> _contentViews = [];
  private readonly Func<LinearLayout, object?, View?> _getItemView;
  private bool _disposed;

  public TabControlHost(Context context, TabControl dataContext, Func<LinearLayout, object?, View?> getItemView) : base(context) {
    _dataContext = dataContext;
    _getItemView = getItemView;
    Orientation = Orientation.Vertical;
    SetBackgroundResource(Resource.Color.c_static_ba);

    _adapter = new TabControlHostHeaderAdapter(dataContext);

    _tabHeaders = new(context) { ScrollBarStyle = ScrollbarStyles.OutsideOverlay };
    _tabHeaders.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Horizontal, false));
    _tabHeaders.SetPadding(context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding));
    _tabHeaders.SetAdapter(_adapter);

    _tabContent = new(context);

    AddView(_tabHeaders, new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
    AddView(_tabContent, new LayoutParams(ViewGroup.LayoutParams.MatchParent, 0, 1));

    dataContext.Tabs.CollectionChanged += _onTabsChanged;
    dataContext.PropertyChanged += _onDataContextPropertyChanged;
    _updateContent();
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _dataContext.Tabs.CollectionChanged -= _onTabsChanged;
      _dataContext.PropertyChanged -= _onDataContextPropertyChanged;

      foreach (var view in _contentViews.Values) {
        _tabContent.RemoveView(view);
        view.Dispose();
      }
      _contentViews.Clear();

      _adapter.Dispose();
      _tabHeaders.Dispose();
      _tabContent.Dispose();
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

  private void _onDataContextPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
    if (e.Is(nameof(TabControl.Selected))) _updateContent();
  }

  private void _updateContent() {
    if (_dataContext.Selected is not { } selectedItem) {
      _adapter.NotifyDataSetChanged();
      return;
    }

    if (!_contentViews.TryGetValue(selectedItem, out var view)) {
      var container = new LinearLayout(Context!);
      view = _getItemView(container, selectedItem.Data);
      if (view != null) {
        container.AddView(view, new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        _contentViews[selectedItem] = container;
      }
    }

    if (view != null)
      foreach (var kvp in _contentViews) {
        kvp.Value.Visibility = kvp.Key == selectedItem ? ViewStates.Visible : ViewStates.Invisible;
        if (kvp.Value.Parent == null)
          _tabContent.AddView(kvp.Value, new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
      }

    _adapter.NotifyDataSetChanged();
  }
}