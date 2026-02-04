using Android.Views;

namespace MH.UI.Android.Controls;

public interface ICollectionViewItemContent {
  View View { get; }
  void Bind(object item);
  void Unbind();
}