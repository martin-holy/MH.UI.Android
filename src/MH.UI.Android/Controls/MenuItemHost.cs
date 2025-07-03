using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls;

public class MenuItemHost : LinearLayout {
  private ImageView _icon = null!;
  private TextView _text = null!;

  public MenuItem? DataContext { get; private set; }

  public MenuItemHost(Context context) : base(context) => _initialize(context);
  public MenuItemHost(Context context, IAttributeSet attrs) : base(context, attrs) => _initialize(context);
  protected MenuItemHost(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) => _initialize(Context!);

  private void _initialize(Context context) {
    LayoutInflater.From(context)!.Inflate(Resource.Layout.menu_item, this, true);
    LayoutParameters = new ViewGroup.LayoutParams(
      ViewGroup.LayoutParams.MatchParent,
      context.Resources!.GetDimensionPixelSize(Resource.Dimension.menu_item_height));
    Orientation = Orientation.Horizontal;
    SetBackgroundResource(Resource.Color.c_static_ba);
    _icon = FindViewById<ImageView>(Resource.Id.icon)!;
    _text = FindViewById<TextView>(Resource.Id.text)!;
  }

  public MenuItemHost Bind(MenuItem? item) {
    DataContext = item;
    if (item == null) return this;
    _icon.SetImageDrawable(Icons.GetIcon(Context, item.Icon));
    _text.SetText(item.Text, TextView.BufferType.Normal);
    return this;
  }
}