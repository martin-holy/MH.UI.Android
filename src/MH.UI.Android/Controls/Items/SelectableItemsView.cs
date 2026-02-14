using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.Utils;
using MH.Utils.Collections;
using System;

namespace MH.UI.Android.Controls.Items;

public class SelectableItemsView<T> : ItemsViewBase<T> where T : class {
  internal static readonly Java.Lang.Object _selectionPayload = new Java.Lang.String("selection");

  public SelectionManager<T> Selection { get; }

  public SelectableItemsView(Context context, Func<Context, View> createItemView) : base(context) {
    ItemsAdapter = new SelectableItemsAdapter<T>(this, createItemView);
    SetLayoutManager(new LinearLayoutManager(context));
    SetAdapter(ItemsAdapter);

    Selection = new SelectionManager<T>(ReferenceEqualityComparer<T>.Instance);
    Selection.ItemSelectionChanged += (item, _) => _notifyItemSelectionChange(item);
    Selection.Cleared += items => {
      foreach (var item in items) {
        _notifyItemSelectionChange(item);
      }
    };
  }

  private void _notifyItemSelectionChange(T item) {
    var index = _items!.IndexOf(item);
    if (index >= 0)
      ItemsAdapter?.NotifyItemChanged(index, _selectionPayload);
  }

  internal override void HandleItemClick(T item) {
    base.HandleItemClick(item);
    Selection.Toggle(item);
  }
}