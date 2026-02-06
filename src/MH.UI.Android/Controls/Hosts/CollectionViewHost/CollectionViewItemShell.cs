using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Controls;
using MH.Utils;
using MH.Utils.Interfaces;
using System;

namespace MH.UI.Android.Controls.Hosts.CollectionViewHost;

public class CollectionViewItemShell : FrameLayout {
  private readonly View _border;
  private readonly Action<ISelectable> _onClickAction;
  private readonly Action<ISelectable> _onLongClickAction;
  private ISelectable? _dataContext;
  private IDisposable? _isSelectedBinding;
  private ICollectionViewItemContent _content;

  public int ShellHeight { get; private set; }

  public CollectionViewItemShell(
    Context context,
    ICollectionViewItemContent content,
    Action<ISelectable> onClick,
    Action<ISelectable> onLongClick) : base(context) {

    _content = content;
    _onClickAction = onClick;
    _onLongClickAction = onLongClick;

    Clickable = true;
    LongClickable = true;
    Focusable = true;
    Click += _onClick;
    LongClick += _onLongClick;

    _border = new(context);
    _border.SetBackgroundResource(Resource.Drawable.collection_view_item_selector);

    AddView(content.View);
    AddView(_border);
  }

  private void _onClick(object? sender, EventArgs e) {
    if (_dataContext != null) _onClickAction(_dataContext);
  }

  private void _onLongClick(object? sender, LongClickEventArgs e) {
    if (_dataContext != null) _onLongClickAction(_dataContext);
  }

  public void Bind(ISelectable dataContext, int itemWidth, int itemHeight) {
    _dataContext = dataContext;
    _isSelectedBinding?.Dispose();
    _isSelectedBinding = _border.Bind(dataContext, nameof(ISelectable.IsSelected), x => x.IsSelected, (t, p) => t.Selected = p);

    var borderSize = CollectionView.ItemBorderSize * 2;
    var oldItemWidth = (_border.LayoutParameters?.Width ?? 0) - borderSize;
    var oldItemHeight = (_border.LayoutParameters?.Height ?? 0) - borderSize;
    ShellHeight = itemHeight + borderSize;
    if (oldItemWidth != itemWidth || oldItemHeight != itemHeight) {
      _content.View.LayoutParameters = new LayoutParams(itemWidth, itemHeight).WithMargin(CollectionView.ItemBorderSize);
      _border.LayoutParameters = new LayoutParams(itemWidth + borderSize, ShellHeight);
    }

    _content.Bind(dataContext);
  }

  public void Unbind() {
    _isSelectedBinding?.Dispose();
    _isSelectedBinding = null;
    _content.Unbind();
  }
}