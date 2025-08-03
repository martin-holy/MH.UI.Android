using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace MH.UI.Android.Controls;

public class IconButton : ImageButton {
  public IconButton(Context context) : this(context, null, Resource.Attribute.iconButtonStyle) { }
  public IconButton(Context context, IAttributeSet? attrs) : this(context, attrs, Resource.Attribute.iconButtonStyle) { }
  public IconButton(Context context, IAttributeSet? attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { }
  protected IconButton(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
}