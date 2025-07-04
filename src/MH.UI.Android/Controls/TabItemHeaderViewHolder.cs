using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils.Interfaces;
using System;

namespace MH.UI.Android.Controls;

public class TabItemHeaderViewHolder : RecyclerView.ViewHolder {
  private readonly ImageView _icon;
  private readonly TextView _name;

  public IListItem? Item { get; private set; }
  public Action<IListItem>? SelectedAction { get; set; }

  public TabItemHeaderViewHolder(View itemView) : base(itemView) {
    itemView.Click += _onContainerClick;
    _icon = itemView.FindViewById<ImageView>(Resource.Id.icon)!;
    _name = itemView.FindViewById<TextView>(Resource.Id.text)!;
  }

  public static TabItemHeaderViewHolder Create(ViewGroup parent) =>
    new(LayoutInflater.From(parent.Context)!.Inflate(Resource.Layout.tab_item_header, parent, false)!);

  public void Bind(IListItem? item, IconTextVisibility itv) {
    Item = item;
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
  }

  private void _onContainerClick(object? sender, EventArgs e) {
    if (SelectedAction != null && Item != null)
      SelectedAction(Item);
  }
}