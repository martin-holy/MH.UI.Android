using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using System;

namespace MH.UI.Android.Controls.Items;

public class ItemsView<T> : ItemsViewBase<T> {
  public ItemsView(Context context, Func<Context, View> createItemView) : base(context) {
    ItemsAdapter = new ItemsAdapter<T>(this, createItemView);
    SetLayoutManager(new LinearLayoutManager(context));
    SetAdapter(ItemsAdapter);
  }
}