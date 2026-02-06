using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Interfaces;
using MH.Utils.Interfaces;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls.Hosts.CollectionViewHost;

public class CollectionViewItemAdapter : RecyclerView.Adapter {
  private readonly Action<ISelectable> _onClick;
  private readonly Action<ISelectable> _onLongClick;
  private readonly Func<Context, ICollectionViewGroup, ICollectionViewItemContent> _createItemContent;
  private ICollectionViewGroup? _group;
  private IReadOnlyList<ISelectable> _items = Array.Empty<ISelectable>();

  public override int ItemCount => _items.Count;

  public CollectionViewItemAdapter(
    Action<ISelectable> onClick,
    Action<ISelectable> onLongClick,
    Func<Context, ICollectionViewGroup, ICollectionViewItemContent> createItemContent) {

    _onClick = onClick;
    _onLongClick = onLongClick;
    _createItemContent = createItemContent;
  }

  public void Submit(ICollectionViewGroup? group, IReadOnlyList<ISelectable> items) {
    _group = group;
    _items = items;
    NotifyDataSetChanged();
  }

  public override int GetItemViewType(int position) =>
    (int)_requireGroup().ViewMode;

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
    var content = _createItemContent(parent.Context!, _requireGroup());
    var shell = new CollectionViewItemShell(parent.Context!, content, _onClick, _onLongClick);
    return new CollectionViewItemViewHolder(shell);
  }

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
    var group = _requireGroup();
    var item = _items[position];
    var itemWidth = group.GetItemSize(item, true);
    var itemHeight = group.GetItemSize(item, false);
    ((CollectionViewItemViewHolder)holder).Bind(item, itemWidth, itemHeight);
  }

  public override void OnViewRecycled(Java.Lang.Object holder) {
    ((CollectionViewItemViewHolder)holder).Unbind();
    base.OnViewRecycled(holder);
  }

  public override void OnViewDetachedFromWindow(Java.Lang.Object holder) {
    ((CollectionViewItemViewHolder)holder).Unbind();
    base.OnViewDetachedFromWindow(holder);
  }

  private ICollectionViewGroup _requireGroup() =>
    _group ?? throw new InvalidOperationException(
      "CollectionViewItemAdapter.Submit() must be called before the adapter is used.");
}