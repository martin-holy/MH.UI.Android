using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Controls;
using MH.Utils;
using MH.Utils.Interfaces;
using System;

namespace MH.UI.Android.Controls;

public class CollectionViewItem : FrameLayout {
  private readonly View _border;
  private ISelectable? _dataContext;
  private IDisposable? _isSelectedBinding;

  public ISelectable DataContext { get => _dataContext ?? throw new NotImplementedException(); }

  public CollectionViewItem(Context context) : base(context) {
    Clickable = true;
    Focusable = true;

    _border = new(context);
    _border.SetBackgroundResource(Resource.Drawable.collection_view_item_selector);
  }

  public CollectionViewItem Bind(ISelectable dataContext, View itemView, int itemWidth, int itemHeight) {
    _dataContext = dataContext;

    var oldItemView = GetChildAt(0);
    RemoveAllViews();
    oldItemView?.Dispose();

    _isSelectedBinding?.Dispose();
    _isSelectedBinding = _border.Bind(dataContext, nameof(ISelectable.IsSelected), x => x.IsSelected, (t, p) => t.Selected = p);

    AddView(itemView, new LayoutParams(itemWidth, itemHeight).WithMargin(CollectionView.ItemBorderSize));
    AddView(_border, new LayoutParams(
      itemWidth + (CollectionView.ItemBorderSize * 2),
      itemHeight + (CollectionView.ItemBorderSize * 2)));

    return this;
  }
}