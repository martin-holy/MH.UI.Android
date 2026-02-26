using MH.UI.Android.Controls.Hosts.TreeMenuHost;
using MH.UI.Controls;

namespace MH.UI.Android.Controls.Hosts.TreeViewHost;

public interface IAndroidTreeViewHost : ITreeViewHost {
  public TreeView DataContext { get; }
  public TreeMenu? ItemMenu { get; }
}