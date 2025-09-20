using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.ViewModels;
using MH.Utils;
using MH.Utils.BaseClasses;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Orientation = Android.Widget.Orientation;

namespace MH.UI.Android.Views;

public class LogV : LinearLayout {
  private readonly LogVM _dataContext;
  private readonly RecyclerView _list;
  private readonly LogAdapter _adapter;
  private readonly EditText _detail;
  private readonly CheckBox _wrapText;
  private readonly Button _clearBtn;
  private readonly CommandBinding _clearCommandBinding;
  private bool _disposed;

  public LogV(Context context, LogVM dataContext) : base(context) {
    _dataContext = dataContext;
    _adapter = new(dataContext.Items, this);
    Orientation = Orientation.Vertical;

    _list = new(context);
    _list.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Vertical, false));
    _list.SetAdapter(_adapter);

    _detail = new(context) {
      Background = null,
      TextSize = 14,
      Gravity = GravityFlags.Top | GravityFlags.Start,
      KeyListener = null
    };
    _detail.SetTextIsSelectable(true);

    _wrapText = new(context) { Checked = true, Text = "Wrap text" };
    _wrapText.CheckedChange += _wrapTextCheckedChange;

    _clearBtn = new(new ContextThemeWrapper(context, Resource.Style.mh_DialogButton), null, 0) {
      Text = dataContext.ClearCommand.Text
    };
    _clearCommandBinding = new(_clearBtn, dataContext.ClearCommand);

    var margin = context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding);

    var footer = new LinearLayout(context) { Orientation = Orientation.Horizontal };
    footer.AddView(_wrapText, new LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1f));
    footer.AddView(_clearBtn, new LayoutParams(ViewGroup.LayoutParams.WrapContent, DisplayU.DpToPx(32)).WithMargin(margin));

    AddView(_list, new LayoutParams(ViewGroup.LayoutParams.MatchParent, 0, 0.4f));
    AddView(_detail, new LayoutParams(ViewGroup.LayoutParams.MatchParent, 0, 1f).WithMargin(margin));
    AddView(footer, new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));

    _dataContext.Items.CollectionChanged += _onItemsCollectionChanged;
  }

  private void _onItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
    Tasks.Dispatch(_adapter.NotifyDataSetChanged);
    _setDetailText(null);
  }

  internal void HandleItemClick(LogItem? item) =>
    _setDetailText(item?.Detail);

  private void _setDetailText(string? text) {
    _detail.Text = text;

    if (string.IsNullOrEmpty(text))
      _detail.Background = null;
    else
      _detail.SetBackgroundResource(Resource.Drawable.view_border);
  }

  private void _wrapTextCheckedChange(object? sender, CompoundButton.CheckedChangeEventArgs e) {
    _detail.SetSingleLine(!e.IsChecked);
    _detail.SetHorizontallyScrolling(!e.IsChecked);
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _adapter.Dispose();
      _list.Dispose();
      _clearCommandBinding.Dispose();
      _dataContext.Items.CollectionChanged -= _onItemsCollectionChanged;
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  private class LogAdapter(ObservableCollection<LogItem> _items, LogV _logV) : RecyclerView.Adapter {
    public override int ItemCount => _items.Count;

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
      new LogItemViewHolder(parent.Context!, _logV);

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) =>
      ((LogItemViewHolder)holder).Bind(_items[position]);
  }

  private class LogItemViewHolder : RecyclerView.ViewHolder {
    private readonly LinearLayout _container;
    private readonly View _level;
    private readonly TextView _text;

    public LogItem? DataContext { get; private set; }

    public LogItemViewHolder(Context context, LogV logV) : base(_createContainerView(context)) {
      _level = new View(context);
      _text = new TextView(context);
      _container = (LinearLayout)ItemView;
      _container.AddView(_level, new LayoutParams(DisplayU.DpToPx(10), ViewGroup.LayoutParams.MatchParent));
      _container.AddView(_text);
      _container.Click += (_, _) => logV.HandleItemClick(DataContext);
    }

    public void Bind(LogItem? item) {
      DataContext = item;
      if (item == null) return;

      _level.SetBackgroundResource(item.Level switch {
        LogLevel.Info => Resource.Color.c_log_info,
        LogLevel.Warning => Resource.Color.c_log_warning,
        LogLevel.Error => Resource.Color.c_log_error,
        _ => throw new ArgumentOutOfRangeException()
      });

      _text.SetText(item.Title, TextView.BufferType.Normal);
    }

    private static LinearLayout _createContainerView(Context context) {
      var container = new LinearLayout(context) {
        Orientation = Orientation.Horizontal,
        Clickable = true,
        Focusable = true
      };
      container.SetGravity(GravityFlags.CenterVertical);
      container.SetBackgroundResource(Resource.Color.c_static_ba);

      return container;
    }
  }
}