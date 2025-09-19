using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.Utils;
using MH.Utils.BaseClasses;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MH.UI.Android.Views;
public class LogV : LinearLayout {
  private readonly ObservableCollection<LogItem> _items;
  private readonly RecyclerView _recyclerView;
  private readonly LogAdapter _adapter;  
  private bool _disposed;

  public LogV(Context context, ObservableCollection<LogItem> items) : base(context) {
    _items = items;
    _adapter = new LogAdapter(items);
    _recyclerView = new(context) {
      LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
    };
    _recyclerView.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Vertical, false));
    _recyclerView.SetAdapter(_adapter);
    AddView(_recyclerView);

    // TODO LogItem detail

    _items.CollectionChanged += _onItemsCollectionChanged;
  }

  private void _onItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
    Tasks.Dispatch(_adapter.NotifyDataSetChanged);

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _adapter.Dispose();
      _recyclerView.Dispose();
      _items.CollectionChanged -= _onItemsCollectionChanged;
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  private class LogAdapter(ObservableCollection<LogItem> _items) : RecyclerView.Adapter {
    public override int ItemCount => _items.Count;

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
      new LogItemViewHolder(parent.Context!);

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) =>
      ((LogItemViewHolder)holder).Bind(_items[position]);
  }

  private class LogItemViewHolder : RecyclerView.ViewHolder {
    private readonly LinearLayout _container;
    private readonly View _level;
    private readonly TextView _text;

    public LogItem? DataContext { get; private set; }

    public LogItemViewHolder(Context context) : base(_createContainerView(context)) {
      _level = new View(context) {
        LayoutParameters = new ViewGroup.LayoutParams(DisplayU.DpToPx(10), DisplayU.DpToPx(20))
      };
      _text = new TextView(context);
      _container = (LinearLayout)ItemView;
      _container.AddView(_level);
      _container.AddView(_text);
    }

    public void Bind(LogItem? item) {
      DataContext = item;
      if (item == null) return;

      _level.SetBackgroundResource(item.Level switch {
        LogLevel.Info => Resource.Color.c_log_info,
        LogLevel.Warning => Resource.Color.c_log_warning,
        LogLevel.Error => Resource.Color.c_log_error
      });

      _text.SetText(item.Title, TextView.BufferType.Normal);
    }

    private static LinearLayout _createContainerView(Context context) {
      var container = new LinearLayout(context) {
        LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
        Orientation = global::Android.Widget.Orientation.Horizontal,
        Clickable = true,
        Focusable = true
      };
      container.SetGravity(GravityFlags.CenterVertical);
      container.SetPadding(context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding));
      container.SetBackgroundResource(Resource.Color.c_static_ba);

      return container;
    }
  }
}