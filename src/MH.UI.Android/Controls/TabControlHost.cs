using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Controls;
using MH.Utils.Extensions;
using MH.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace MH.UI.Android.Controls;

public class TabControlHost : LinearLayout {
  private RecyclerView _tabHeaders = null!;
  private FrameLayout _tabContent = null!;
  private TabControl? _dataContext;
  private TabControlHostHeaderAdapter? _adapter;
  private readonly Dictionary<IListItem, View> _contentViews = [];

  public Func<LinearLayout, object?, View?> GetItemView { get; set; } =
    (container, item) => throw new NotImplementedException();

  public TabControl? DataContext {
    get => _dataContext;
    set {
      _updateEvents(_dataContext, value);
      _dataContext = value;
      if (_dataContext == null) return;      
      _adapter = new TabControlHostHeaderAdapter(_dataContext);
      _tabHeaders.SetAdapter(_adapter);
      _updateContent();
    }
  }

  public TabControlHost(Context context) : base(context) => _initialize(context, null);
  public TabControlHost(Context context, IAttributeSet attrs) : base(context, attrs) => _initialize(context, attrs);
  protected TabControlHost(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) => _initialize(Context!, null);

  private void _initialize(Context context, IAttributeSet? attrs) {
    LayoutInflater.From(context)!.Inflate(Resource.Layout.tab_control_host, this, true);
    _tabHeaders = FindViewById<RecyclerView>(Resource.Id.tab_headers)!;
    _tabHeaders.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Horizontal, false));
    _tabContent = FindViewById<FrameLayout>(Resource.Id.tab_content)!;
  }

  private void _updateEvents(TabControl? oldValue, TabControl? newValue) {
    if (oldValue != null) {
      oldValue.Tabs.CollectionChanged -= _onTabsChanged;
      oldValue.PropertyChanged -= _onDataContextPropertyChanged;
    }

    if (newValue != null) {
      newValue.Tabs.CollectionChanged += _onTabsChanged;
      newValue.PropertyChanged += _onDataContextPropertyChanged;
    }
  }

  private void _onTabsChanged(object? sender, NotifyCollectionChangedEventArgs e) {
    if (e.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Reset)
      foreach (var item in e.OldItems?.Cast<IListItem>() ?? []) {
        if (_contentViews.TryGetValue(item, out var view)) {
          _tabContent.RemoveView(view);
          _contentViews.Remove(item);
        }
      }

    _updateContent();
  }

  private void _onDataContextPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
    if (e.Is(nameof(TabControl.Selected)))
      _updateContent();
  }

  private void _updateContent() {
    if (_dataContext?.Selected is not { } selectedItem) {
      _adapter?.NotifyDataSetChanged();
      return;
    }

    if (!_contentViews.TryGetValue(selectedItem, out var view)) {
      var container = new LinearLayout(Context!) {
        LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
      };
      view = GetItemView(container, selectedItem.Data);
      if (view != null) {
        container.AddView(view);
        _contentViews[selectedItem] = container;
      }
    }

    if (view != null) {
      foreach (var kvp in _contentViews) {
        kvp.Value.Visibility = kvp.Key == selectedItem ? ViewStates.Visible : ViewStates.Invisible;
        if (kvp.Value.Parent == null)
          _tabContent.AddView(kvp.Value);
      }
    }

    _adapter?.NotifyDataSetChanged();
  }
}