using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;
using MH.Utils.Extensions;
using MH.Utils.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace MH.UI.Android.Controls.Hosts.TreeViewHost;

public class TreeViewHostAdapter : TreeViewHostAdapterBase {
  private readonly TreeViewHost _host;
  private readonly ITreeItemViewHolderFactory _factory;
  private static readonly Java.Lang.Object _isSelectedPayload = new Java.Lang.String("IsSelected");

  public TreeViewHostAdapter(Context context, TreeViewHost host, ITreeItemViewHolderFactory? factory = null)
    : base(context, host.DataContext.RootHolder) {
    _host = host;
    _factory = factory ?? new DefaultTreeItemViewHolderFactory();
    _flatTree.TreeItemPropertyChangedEvent += _onTreeItemPropertyChanged;
  }

  public override int GetItemViewType(int position) =>
    _factory.GetViewType(Items[position]);

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
    _factory.Create(parent, viewType, _host);

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) =>
    (holder.ItemView as IBindable<FlatTreeItem>)?.Bind(Items[position]);

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position, IList<Java.Lang.Object> payloads) {
    base.OnBindViewHolder(holder, position, payloads);
    if (payloads.Count == 0) return;

    if (holder.ItemView is not FlatTreeItemV view) return;
    var item = Items[position];
    foreach (var payload in payloads)
      if (ReferenceEquals(payload, _isSelectedPayload))
        view.BindIsSelected(item);
  }

  private void _onTreeItemPropertyChanged(ITreeItem item, int index, PropertyChangedEventArgs e) {
    if (_host.DataContext.ShowTreeItemSelection && e.Is(nameof(ISelectable.IsSelected)))
      NotifyItemChanged(index, _isSelectedPayload);
  }
}