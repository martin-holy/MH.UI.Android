using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Interfaces;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls.Recycler;

public class BindableAdapter<T> : RecyclerView.Adapter {
  private readonly Func<IReadOnlyList<T>> _getItems;
  private readonly Func<Context, View> _createView;
  private readonly Func<RecyclerView.LayoutParams> _createLayoutParams;
  private readonly Action<View>? _onViewCreated;
  private readonly Func<View, int, IList<Java.Lang.Object>, bool>? _onPayloadBind;

  public BindableAdapter(
    Func<IReadOnlyList<T>> getItems,
    Func<Context, View> createView,
    Func<RecyclerView.LayoutParams> createLayoutParams,
    Action<View>? onViewCreated = null,
    Func<View, int, IList<Java.Lang.Object>, bool>? onPayloadBind = null) {

    _getItems = getItems;
    _createView = createView;
    _createLayoutParams = createLayoutParams;
    _onViewCreated = onViewCreated;
    _onPayloadBind = onPayloadBind;
  }

  public override int ItemCount => _getItems().Count;

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
    var view = _createView(parent.Context!);
    _onViewCreated?.Invoke(view);
    return new BaseViewHolder(view, _createLayoutParams());
  }

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) =>
    (holder.ItemView as IBindable<T>)?.Rebind(_getItems()[position]);

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position, IList<Java.Lang.Object> payloads) {
    if (payloads.Count == 0) {
      base.OnBindViewHolder(holder, position, payloads);
      return;
    }

    if (_onPayloadBind?.Invoke(holder.ItemView, position, payloads) == true)
      return;

    base.OnBindViewHolder(holder, position, payloads);
  }

  public override void OnViewRecycled(Java.Lang.Object holder) {
    (((RecyclerView.ViewHolder)holder).ItemView as IBindable<T>)?.Unbind();
    base.OnViewRecycled(holder);
  }
}