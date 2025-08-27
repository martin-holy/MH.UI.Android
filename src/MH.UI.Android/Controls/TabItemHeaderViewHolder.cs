using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils.Extensions;
using MH.Utils.Interfaces;
using System.ComponentModel;
using System.Windows.Input;

namespace MH.UI.Android.Controls;

public class TabItemHeaderViewHolder : RecyclerView.ViewHolder {
  protected readonly LinearLayout _container;
  private readonly IconView _icon;
  private readonly TextView _name;
  private bool _disposed;
  private readonly CommandBinding _selectItemCommandBinding;

  public IListItem? DataContext { get; private set; }

  public TabItemHeaderViewHolder(Context context, ICommand selectItemCommand) : base(_createContainerView(context)) {
    _icon = new(context);
    _name = new(context);
    _container = (LinearLayout)ItemView;
    _container.AddView(_icon);
    _container.AddView(_name);
    _selectItemCommandBinding = new CommandBinding(_container, selectItemCommand);
  }

  public void Bind(IListItem? item, IconTextVisibility itv) {
    _setDataContext(DataContext, item);
    if (item == null) return;

    var isIconVisible = itv.HasFlag(IconTextVisibility.Icon);
    var isTextVisible = itv.HasFlag(IconTextVisibility.Text) && !string.IsNullOrEmpty(item.Name);

    _icon.Visibility = isIconVisible ? ViewStates.Visible : ViewStates.Gone;
    _icon.SetImageDrawable(isIconVisible ? Icons.GetIcon(_icon.Context, item.Icon) : null);

    _name.Visibility = isTextVisible ? ViewStates.Visible : ViewStates.Gone;
    _name.Text = item.Name;
    _name.SetPadding(isIconVisible
      ? ItemView.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding)
      : 0, 0, 0, 0);

    ItemView.Selected = item.IsSelected;
    _selectItemCommandBinding.Parameter = item;
  }

  private void _setDataContext(IListItem? oldValue, IListItem? newValue) {
    if (oldValue != null) oldValue.PropertyChanged -= _onDataContextPropertyChanged;
    if (newValue != null) newValue.PropertyChanged += _onDataContextPropertyChanged;
  }

  private void _onDataContextPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    if (sender is not IListItem listItem) return;
    if (e.Is(nameof(IListItem.Name)))
      _name.Text = listItem.Name;
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _selectItemCommandBinding.Dispose();
      _setDataContext(DataContext, null);
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  private static LinearLayout _createContainerView(Context context) {
    var container = new LinearLayout(context) {
      LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent),
      Orientation = Orientation.Horizontal,
      Clickable = true,
      Focusable = true
    };
    container.SetGravity(GravityFlags.CenterVertical);
    container.SetPadding(context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding));
    container.SetBackgroundResource(Resource.Drawable.tab_item_header_background);

    return container;
  }
}