using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Controls;
using System;

namespace MH.UI.Android.Controls;

public class TreeViewHost : TreeViewHostBase<TreeView, TreeViewHostAdapter>, ITreeViewHost {
  public TreeViewHost(Context context, TreeView dataContext, Func<View, object?, PopupWindow?> itemMenuFactory)
    : base(context, dataContext, itemMenuFactory) {
    DataContext.Host = this;
    _adapter = new TreeViewHostAdapter(context, this);
    _recyclerView.SetAdapter(_adapter);
  }
}