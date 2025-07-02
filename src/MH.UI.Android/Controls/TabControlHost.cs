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
  private TabControlHostHeaderAdapter? _adapter;
  private readonly Dictionary<IListItem, View> _contentViews = [];

  public TabControl? DataContext { get; private set; }
  public Func<LinearLayout, object?, View?> GetItemView { get; set; } =
    (container, item) => throw new NotImplementedException();

  public TabControlHost(Context context) : base(context) => _initialize(context);
  public TabControlHost(Context context, IAttributeSet attrs) : base(context, attrs) => _initialize(context);
  protected TabControlHost(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) => _initialize(Context!);

  private void _initialize(Context context) {
    LayoutInflater.From(context)!.Inflate(Resource.Layout.tab_control_host, this, true);
    _tabHeaders = FindViewById<RecyclerView>(Resource.Id.tab_headers)!;
    _tabHeaders.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Horizontal, false));
    _tabContent = FindViewById<FrameLayout>(Resource.Id.tab_content)!;
  }

  public TabControlHost Bind(TabControl? dataContext) {
    _updateEvents(DataContext, dataContext);
    DataContext = dataContext;
    if (DataContext == null) return this;      
    _adapter = new TabControlHostHeaderAdapter(DataContext);
    _tabHeaders.SetAdapter(_adapter);
    _updateContent();
    return this;
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
    if (DataContext?.Selected is not { } selectedItem) {
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