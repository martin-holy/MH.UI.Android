using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Utils;
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

  public void Bind(IListItem? item) {
    Item = item;
    if (item == null) return;

    _name.Visibility = item.IsNameHidden || string.IsNullOrEmpty(item.Name) ? ViewStates.Gone : ViewStates.Invisible;
    _name.Text = item.Name;
    _icon.Visibility = item.IsIconHidden ? ViewStates.Gone : ViewStates.Visible;
    _icon.SetImageDrawable(item.IsIconHidden ? null : Icons.GetIcon(_icon.Context, item.Icon));
    ItemView.Selected = item.IsSelected;
  }

  public static TabItemHeaderViewHolder Create(ViewGroup parent) =>
    new(LayoutInflater.From(parent.Context)!.Inflate(Resource.Layout.tab_item_header, parent, false)!);

  private void _onContainerClick(object? sender, EventArgs e) {
    if (SelectedAction != null && Item != null)
      SelectedAction(Item);
  }
}