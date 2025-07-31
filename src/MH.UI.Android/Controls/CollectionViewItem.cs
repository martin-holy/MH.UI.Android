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

  public ISelectable DataContext { get => _dataContext ?? throw new NotImplementedException(); }

  public CollectionViewItem(Context context) : base(context) {
    LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
    Clickable = true;
    Focusable = true;

    _border = new View(context);
    _border.SetBackgroundResource(Resource.Drawable.collection_view_item_selector);
  }

  public CollectionViewItem Bind(ISelectable dataContext, View itemView, int itemWidth, int itemHeight) {
    _updateEvents(_dataContext, dataContext);
    _dataContext = dataContext;
    RemoveAllViews();

    itemView.LayoutParameters = new MarginLayoutParams(itemWidth, itemHeight);
    itemView.SetMargin(CollectionView.ItemBorderSize);

    _border.LayoutParameters = new LayoutParams(
      itemWidth + (CollectionView.ItemBorderSize * 2),
      itemHeight + (CollectionView.ItemBorderSize * 2));

    AddView(itemView);
    AddView(_border);
    return this;
  }

  private void _updateEvents(ISelectable? oldValue, ISelectable? newValue) {
    if (oldValue != null) oldValue.PropertyChanged -= _onDataContextPropertyChanged;
    if (newValue != null) newValue.PropertyChanged += _onDataContextPropertyChanged;
  }

  private void _onDataContextPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    if (e.Is(nameof(ISelectable.IsSelected)))
      _border.Selected = DataContext.IsSelected;
  }
}