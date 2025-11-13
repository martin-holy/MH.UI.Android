using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils;
using MH.Utils.Interfaces;

namespace MH.UI.Android.Controls;

public class TabItemHeaderViewHolder : RecyclerView.ViewHolder {
  private readonly TabControlHost _tabControlHost;
  private readonly LinearLayout _container;
  private readonly ImageView _icon;
  private readonly TextView _name;
  private readonly CommandBinding _selectItemCommandBinding;
  private ViewBinder<TextView, string>? _nameBinding;

  public IListItem? DataContext { get; private set; }

  public TabItemHeaderViewHolder(Context context, TabControlHost tabControlHost) : base(_createContainerView(context)) {
    _tabControlHost = tabControlHost;
    _container = (LinearLayout)ItemView;

    if (tabControlHost.DataContext.TabStrip.IconTextVisibility.HasFlag(IconTextVisibility.Both)) {
      _icon = new IconButton(context)
        .WithClickAction(this, (o, s) => o._tabControlHost.ItemMenu?.ShowItemMenu(s, o.DataContext));
      _icon.SetPadding(0);
      _container.AddView(_icon);
    }
    else {
      _icon = new IconView(context);
      _container.AddView(_icon, new LinearLayout.LayoutParams(DimensU.IconSize, DimensU.IconSize)
        .WithMargin(DimensU.Spacing, DimensU.Spacing, 0, DimensU.Spacing));
    }
    
    _name = new TextView(context);
    _container.AddView(_name);
    _selectItemCommandBinding = _container.Bind(tabControlHost.DataContext.SelectTabCommand);
  }

  public void Bind(IListItem? item, IconTextVisibility itv) {
    DataContext = item;
    if (item == null) return;

    var isIconVisible = itv.HasFlag(IconTextVisibility.Icon);
    var isTextVisible = itv.HasFlag(IconTextVisibility.Text) && !string.IsNullOrEmpty(item.Name);

    _icon.Visibility = isIconVisible ? ViewStates.Visible : ViewStates.Gone;
    _icon.SetImageDrawable(isIconVisible ? Icons.GetIcon(_icon.Context, item.Icon) : null);

    _name.Visibility = isTextVisible ? ViewStates.Visible : ViewStates.Gone;
    _name.SetPadding(isIconVisible ? DimensU.Spacing : 0);
    _nameBinding?.Dispose();
    _nameBinding = new(_name, (v, val) => v.Text = val);
    _nameBinding.Bind(item, x => x.Name);

    ItemView.Selected = item.IsSelected;
    _selectItemCommandBinding.Parameter = item;
  }

  private static LinearLayout _createContainerView(Context context) {
    var container = new LinearLayout(context) {
      LayoutParameters = new RecyclerView.LayoutParams(LPU.Wrap, LPU.Wrap),
      Orientation = Orientation.Horizontal,
      Clickable = true,
      Focusable = true
    };
    container.SetGravity(GravityFlags.CenterVertical);
    container.SetPadding(0, 0, DimensU.Spacing, 0);
    container.SetBackgroundResource(Resource.Drawable.tab_item_header_background);

    return container;
  }
}