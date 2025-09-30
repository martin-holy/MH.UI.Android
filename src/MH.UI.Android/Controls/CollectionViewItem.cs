using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Controls;
using MH.Utils.Extensions;
using MH.Utils.Interfaces;
using System;
using System.ComponentModel;

namespace MH.UI.Android.Controls;

public class CollectionViewItem : FrameLayout {
  private readonly View _border;
  private ISelectable? _dataContext;
  private bool _disposed;

  public ISelectable DataContext { get => _dataContext ?? throw new NotImplementedException(); }

  public CollectionViewItem(Context context) : base(context) {
    Clickable = true;
    Focusable = true;

    _border = new(context);
    _border.SetBackgroundResource(Resource.Drawable.collection_view_item_selector);
  }

  public CollectionViewItem Bind(ISelectable dataContext, View itemView, int itemWidth, int itemHeight) {
    _setDataContext(_dataContext, dataContext);

    var oldItemView = GetChildAt(0);
    RemoveAllViews();
    oldItemView?.Dispose();

    AddView(itemView, new LayoutParams(itemWidth, itemHeight).WithMargin(CollectionView.ItemBorderSize));
    AddView(_border, new LayoutParams(
      itemWidth + (CollectionView.ItemBorderSize * 2),
      itemHeight + (CollectionView.ItemBorderSize * 2)));

    return this;
  }

  private void _setDataContext(ISelectable? oldValue, ISelectable? newValue) {
    if (oldValue != null) oldValue.PropertyChanged -= _onDataContextPropertyChanged;
    if (newValue != null) newValue.PropertyChanged += _onDataContextPropertyChanged;
    _dataContext = newValue;
  }

  private void _onDataContextPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    if (e.Is(nameof(ISelectable.IsSelected)))
      _border.Selected = DataContext.IsSelected;
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _setDataContext(_dataContext, null);
    }
    _disposed = true;
    base.Dispose(disposing);
  }
}