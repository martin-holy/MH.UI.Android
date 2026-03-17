using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Utils;
using MH.Utils;
using MH.Utils.Collections;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls.Items;

public class SelectableItemsView<T> : ItemsViewBase<T> where T : class {
  internal static readonly Java.Lang.Object _selectionPayload = new Java.Lang.String("selection");

  public SelectionManager<T> Selection { get; }

  public SelectableItemsView(Context context, IReadOnlyList<T> items, Func<Context, View> createItemView) : base(context, items) {
    ItemsAdapter = new SelectableItemsAdapter<T>(this, createItemView, () => new(LPU.Match, LPU.Wrap), _onViewCreated);
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
    if (_items is not IList<T> list) return;
    var index = list.IndexOf(item);
    if (index >= 0)
      ItemsAdapter?.NotifyItemChanged(index, _selectionPayload);
  }

  internal override void HandleItemClick(T item) {
    base.HandleItemClick(item);
    Selection.Toggle(item);
  }
}