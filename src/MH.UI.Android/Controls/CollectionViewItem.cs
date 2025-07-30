using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Controls;
using MH.UI.Interfaces;
using MH.Utils.Extensions;
using MH.Utils.Interfaces;
using System;
using System.ComponentModel;

namespace MH.UI.Android.Controls;

public class CollectionViewItem : FrameLayout {
  private View _border = null!;
  private CollectionViewHost _host = null!;
  private ICollectionViewRow _row = null!;
  private ISelectable? _dataContext;

  public ISelectable DataContext { get => _dataContext ?? throw new NotImplementedException(); }

  public CollectionViewItem(Context context) : base(context) => _initialize(context);
  public CollectionViewItem(Context context, IAttributeSet attrs) : base(context, attrs) => _initialize(context);
  protected CollectionViewItem(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) => _initialize(Context!);

  private void _initialize(Context context) {
    LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
    Clickable = true;
    Focusable = true;
    Click += _onClick;
    LongClick += _onLongClick;

    _border = new View(context);
    _border.SetBackgroundResource(Resource.Drawable.collection_view_item_selector);
  }

  public CollectionViewItem Bind(CollectionViewHost host, ICollectionViewRow row, ISelectable dataContext, View itemView) {
    _updateEvents(_dataContext, dataContext);
    _dataContext = dataContext;
    if (_dataContext == null) return this;   

    _host = host;
    _row = row;
    RemoveAllViews();

    if (row.Parent is not ICollectionViewGroup { } group) return this;

    var itemWidth = group.GetItemSize(dataContext, true);
    var itemHeight = group.GetItemSize(dataContext, false);
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
    if (e.Is(nameof(ISelectable.IsSelected))) {
      _border.Selected = DataContext.IsSelected;
      _border.Invalidate();
    }
  }

  private void _onClick(object? sender, EventArgs e) {
    if (_host.ViewModel is not { } vm) return;
    _host.IsMultiSelectOn = false;
    if (vm.CanSelect) vm.SelectItem(_row, DataContext, false, false);
    if (vm.CanOpen) vm.OpenItem(DataContext);
  }

  private void _onLongClick(object? sender, LongClickEventArgs e) {
    if (_host.ViewModel is not { } vm) return;
    var isCtrlOn = _host.IsMultiSelectOn;
    _host.IsMultiSelectOn = true;
    if (vm.CanSelect) vm.SelectItem(_row, DataContext, isCtrlOn, false);
  }
}