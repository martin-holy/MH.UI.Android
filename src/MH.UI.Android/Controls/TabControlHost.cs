using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils;
using MH.Utils.BaseClasses;
using MH.Utils.Interfaces;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace MH.UI.Android.Controls;

public class TabControlHost : LinearLayout {
  private readonly RecyclerView _tabHeaders;
  private readonly FrameLayout _tabContent;
  private readonly TabControlHostHeaderAdapter _adapter;
  private readonly Dictionary<IListItem, View> _contentViews = [];
  private bool _disposed;

  public TabControl DataContext { get; }
  public TreeMenu ItemMenu { get; }

  public TabControlHost(Context context, TabControl dataContext) : base(context) {
    DataContext = dataContext;
    ItemMenu = new TreeMenu(context, dataContext.ItemMenuFactory);
    Orientation = Orientation.Vertical;
    SetBackgroundResource(Resource.Color.c_static_ba);

    _adapter = new TabControlHostHeaderAdapter(this);

    _tabHeaders = new RecyclerView(context) { ScrollBarStyle = ScrollbarStyles.OutsideOverlay };
    _tabHeaders.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Horizontal, false));
    _tabHeaders.SetPadding(DimensU.Spacing);
    _tabHeaders.SetAdapter(_adapter);

    _tabContent = new FrameLayout(context);

    AddView(_tabHeaders, new LayoutParams(LPU.Match, LPU.Wrap));
    AddView(_tabContent, new LayoutParams(LPU.Match, 0, 1));

    this.Bind(DataContext, nameof(TabControl.Tabs), x => x.Tabs, (_, _, e) => _onTabsChanged(null, e));
    this.Bind(DataContext, nameof(TabControl.Selected), x => x.Selected, (_, _) => _updateContent());
    _updateContent();
  }

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