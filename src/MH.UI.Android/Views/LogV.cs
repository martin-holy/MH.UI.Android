using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Controls.Items;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.ViewModels;
using MH.Utils;
using MH.Utils.BaseClasses;
using System.Collections.Specialized;

namespace MH.UI.Android.Views;

public class LogV : LinearLayout {
  private readonly LogVM _dataContext;
  private readonly SelectableItemsView<LogItem> _items;
  private readonly EditText _detail;
  private readonly CheckBox _wrapText;
  private readonly Button _clearBtn;
  private bool _disposed;

  public LogV(Context context, LogVM dataContext) : base(context) {
    _dataContext = dataContext;
    Orientation = Orientation.Vertical;

    _items = new(context, x => new LogItemV(x));
    _items.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Vertical, false));
    _items.BindItems(dataContext.Items);
    _items.ItemClickedEvent += item => _setDetailText(item?.Detail);

    _detail = new(context) {
      Background = null,
      TextSize = 14,
      Gravity = GravityFlags.Top | GravityFlags.Start,
      KeyListener = null
    };
    _detail.SetTextIsSelectable(true);

    _wrapText = new(context) { Checked = true, Text = "Wrap text" };
    _wrapText.CheckedChange += _wrapTextCheckedChange;

    _clearBtn = new(new ContextThemeWrapper(context, Resource.Style.mh_DialogButton), null, 0);
    _clearBtn.Bind(dataContext.ClearCommand);

    var footer = new LinearLayout(context) { Orientation = Orientation.Horizontal };
    footer.AddView(_wrapText, new LayoutParams(0, LPU.Wrap, 1f));
    footer.AddView(_clearBtn, new LayoutParams(LPU.Wrap, DisplayU.DpToPx(32)).WithMargin(DimensU.Spacing));

    AddView(_items, new LayoutParams(LPU.Match, 0, 0.4f).WithMargin(DimensU.Spacing));
    AddView(_detail, new LayoutParams(LPU.Match, 0, 1f).WithMargin(DimensU.Spacing));
    AddView(footer, new LayoutParams(LPU.Match, LPU.Wrap));

    _dataContext.Items.CollectionChanged += _onItemsCollectionChanged;
  }

  private void _onItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
    _setDetailText(null);
  }

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
      _dataContext.Items.CollectionChanged -= _onItemsCollectionChanged;
      _wrapText.CheckedChange -= _wrapTextCheckedChange;
    }
    _disposed = true;
    base.Dispose(disposing);
  }
}