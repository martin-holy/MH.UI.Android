using Android.Views;

namespace MH.UI.Android.Controls.Hosts.CollectionViewHost;

public interface ICollectionViewItemContent {
  View View { get; }
  void Bind(object item);
  void Unbind();
}