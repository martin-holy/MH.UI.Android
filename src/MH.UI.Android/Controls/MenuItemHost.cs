using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls;

public class MenuItemHost : LinearLayout {
  private readonly ImageView _icon;
  private readonly TextView _text;

  public MenuItem DataContext { get; }

  public MenuItemHost(Context context, MenuItem item) : base(context) {
    DataContext = item;
    LayoutParameters = new ViewGroup.LayoutParams(
      ViewGroup.LayoutParams.MatchParent,
      context.Resources!.GetDimensionPixelSize(Resource.Dimension.menu_item_height));
    Orientation = Orientation.Horizontal;
    SetBackgroundResource(Resource.Color.c_static_ba);

    _icon = _createIconView(context);
    _icon.SetImageDrawable(Icons.GetIcon(Context, item.Icon));
    _text = _createTextView(context);
    _text.SetText(item.Text, TextView.BufferType.Normal);
    AddView(_icon);
    AddView(_text);
  }

  private static ImageView _createIconView(Context context) =>
    new(context) {
      LayoutParameters = new LinearLayout.LayoutParams(DisplayU.DpToPx(24), DisplayU.DpToPx(24)) {
        MarginStart = DisplayU.DpToPx(8),
        Gravity = GravityFlags.CenterVertical
      }
    };

  private static TextView _createTextView(Context context) {
    var textView = new TextView(context) {
      LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent) {
        MarginStart = DisplayU.DpToPx(8),
        Gravity = GravityFlags.CenterVertical
      }
    };
    textView.SetTextColor(new Color(context.Resources.GetColor(Resource.Color.c_static_fo, context.Theme)));
    textView.SetSingleLine(true);

    return textView;
  }
}