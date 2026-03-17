using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Controls.Recycler;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls.Items;

public class SelectableItemsAdapter<T> : BindableAdapter<T> where T : class {
  private readonly SelectableItemsView<T> _owner;

  public SelectableItemsAdapter(
    SelectableItemsView<T> owner,
    Func<Context, View> createItemView,
    Func<RecyclerView.LayoutParams> createLayoutParams,
    Action<View> onViewCreated)
    : base(() => owner._items, createItemView, createLayoutParams, onViewCreated) {
      _owner = owner;
    }

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position, IList<Java.Lang.Object> payloads) {
    if (payloads.Count == 0) {
      base.OnBindViewHolder(holder, position, payloads);
      return;
    }

    foreach (var payload in payloads) {
      if (Equals(payload, SelectableItemsView<T>._selectionPayload))
        holder.ItemView.Selected = _owner.Selection.IsSelected(_owner._items![position]);
    }
  }
}