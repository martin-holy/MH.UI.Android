using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls.Items;

public class SelectableItemsAdapter<T> : ItemsAdapter<T> where T : class {
  public SelectableItemsAdapter(SelectableItemsView<T> owner, Func<Context, View> createItemView) : base(owner, createItemView) { }

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position, IList<Java.Lang.Object> payloads) {
    if (payloads.Count == 0) {
      base.OnBindViewHolder(holder, position, payloads);
      return;
    }

    var selection = ((SelectableItemsView<T>)_owner).Selection;

    foreach (var payload in payloads) {
      if (Equals(payload, SelectableItemsView<T>._selectionPayload))
        holder.ItemView.Selected = selection.IsSelected(_owner._items![position]);
    }
  }
}