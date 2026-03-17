using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Controls.Recycler;
using MH.UI.Android.Utils;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls.Items;

public class ItemsView<T> : ItemsViewBase<T> {
  public ItemsView(Context context, IReadOnlyList<T> items, Func<Context, View> createItemView) : base(context, items) {
    ItemsAdapter = new BindableAdapter<T>(() => this._items, createItemView, () => new(LPU.Match, LPU.Wrap));
    SetLayoutManager(new LinearLayoutManager(context));
    SetAdapter(ItemsAdapter);
  }
}