using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Controls.Hosts.TreeViewHost;
using MH.UI.Android.Controls.Recycler;
using MH.UI.Android.Utils;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;
using MH.Utils.Extensions;
using MH.Utils.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace MH.UI.Android.Controls;

public interface ITreeItemViewHolderFactory {
  public int GetViewType(FlatTreeItem item);
  public RecyclerView.ViewHolder Create(ViewGroup parent, int viewType, IAndroidTreeViewHost host);
}

internal sealed class DefaultTreeItemViewHolderFactory : ITreeItemViewHolderFactory {
  public int GetViewType(FlatTreeItem item) => 0;

  public RecyclerView.ViewHolder Create(ViewGroup parent, int viewType, IAndroidTreeViewHost host) =>
    new BaseViewHolder(new FlatTreeItemV(parent.Context!, (TreeViewHost)host), new(LPU.Match, LPU.Wrap));
}

public class TreeViewHostAdapter : TreeViewHostAdapterBase {
  private readonly TreeViewHost _host;
  private readonly ITreeItemViewHolderFactory _factory;

  public TreeViewHostAdapter(Context context, TreeViewHost host, ITreeItemViewHolderFactory? factory = null)
    : base(context, host.DataContext.RootHolder) {
    _host = host;
    _factory = factory ?? new DefaultTreeItemViewHolderFactory();
  }

  public override int GetItemViewType(int position) =>
    _factory.GetViewType(_items[position]);

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
    _factory.Create(parent, viewType, _host);

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) =>
    (holder.ItemView as IBindable<FlatTreeItem>)?.Bind(_items[position]);

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position, IList<Java.Lang.Object> payloads) {
    if (payloads.Count == 0) {
      base.OnBindViewHolder(holder, position, payloads);
      return;
    }

    var item = _items[position];
    if (holder.ItemView is not FlatTreeItemV view) return;
    foreach (var payload in payloads) {
      if (Equals(nameof(ITreeItem.IsSelected), payload.ToString()))
        view.UpdateSelection(item.TreeItem.IsSelected);
    }
  }

  protected override void _onTreeItemPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    base._onTreeItemPropertyChanged(sender, e);

    if (sender is not ITreeItem treeItem) return;
    var index = _findIndexOfTreeItem(treeItem);
    if (index == -1) return;

    if (_host.DataContext.ShowTreeItemSelection && e.Is(nameof(ISelectable.IsSelected)))
      NotifyItemChanged(index, nameof(ITreeItem.IsSelected));
  }
}