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

    _name.Visibility = itv.HasFlag(IconTextVisibility.Text) && !string.IsNullOrEmpty(item.Name)
      ? ViewStates.Visible
      : ViewStates.Gone;
    _name.Text = item.Name;

    _icon.Visibility = itv.HasFlag(IconTextVisibility.Icon) ? ViewStates.Visible : ViewStates.Gone;
    _icon.SetImageDrawable(itv.HasFlag(IconTextVisibility.Icon) ? Icons.GetIcon(_icon.Context, item.Icon) : null);
    
    ItemView.Selected = item.IsSelected;
  }

  private void _onContainerClick(object? sender, EventArgs e) {
    if (SelectedAction != null && Item != null)
      SelectedAction(Item);
  }
}