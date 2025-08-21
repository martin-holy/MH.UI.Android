using Android.Content;
using MH.UI.Controls;
using MH.Utils.BaseClasses;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls;

public class TreeViewHost : TreeViewHostBase<TreeView, TreeViewHostAdapter>, ITreeViewHost {
  public TreeViewHost(Context context, TreeView dataContext, Func<object?, IEnumerable<MenuItem>?> itemMenuFactory)
    : base(context, dataContext, itemMenuFactory) {
    DataContext.Host = this;
    Adapter = new TreeViewHostAdapter(context, this);
    _recyclerView.SetAdapter(Adapter);
  }
}