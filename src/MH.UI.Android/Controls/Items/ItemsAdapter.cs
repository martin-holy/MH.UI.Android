using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using System;

namespace MH.UI.Android.Controls.Items;

public class ItemsAdapter<T> : RecyclerView.Adapter {
  protected readonly ItemsViewBase<T> _owner;
  private readonly Func<Context, View> _createItemView;

  public override int ItemCount => _owner._items?.Count ?? 0;

  public ItemsAdapter(ItemsViewBase<T> owner, Func<Context, View> createItemView) {
    _owner = owner;
    _createItemView = createItemView;
  }

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
    new ItemViewHolder<T>(_createItemView(parent.Context!), _owner);

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) =>
    ((ItemViewHolder<T>)holder).Bind(_owner._items![position]);
}