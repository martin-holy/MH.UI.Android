using Android.Views;
using MH.UI.Interfaces;

namespace MH.UI.Android.Controls.Hosts.CollectionViewHost;

public interface ICollectionViewItemContent : IBindable<object> {
  View View { get; }
}