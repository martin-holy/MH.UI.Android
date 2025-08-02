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
    SetGravity(GravityFlags.CenterVertical);

    _icon = _createIconView(context);
    _icon.SetImageDrawable(Icons.GetIcon(Context, item.Icon));
    _text = _createTextView(context);
    _text.SetText(item.Text, TextView.BufferType.Normal);
    AddView(_icon);
    AddView(_text);

    if (item.Items.Count > 0)
      AddView(_createSubMenuItemArrow(context));
  }

  private static ImageView _createIconView(Context context) =>
    new(context) {
      LayoutParameters = new LinearLayout.LayoutParams(DisplayU.DpToPx(24), DisplayU.DpToPx(24)) {
        MarginStart = DisplayU.DpToPx(8),
        Weight = 0
      }
    };

  private static TextView _createTextView(Context context) {
    var textView = new TextView(context) {
      LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent) {
        MarginStart = DisplayU.DpToPx(8),
        Weight = 1
      }
    };
    textView.SetTextColor(new Color(context.Resources!.GetColor(Resource.Color.c_static_fo, context.Theme)));
    textView.SetSingleLine(true);

    return textView;
  }

  private static ImageView _createSubMenuItemArrow(Context context) {
    var arrow = new ImageView(context) {
      LayoutParameters = new LinearLayout.LayoutParams(DisplayU.DpToPx(12), DisplayU.DpToPx(12)) {
        Weight = 0,
        RightMargin = context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding)
      }
    };
    arrow.SetImageResource(Resource.Drawable.sub_menu_item_arrow);

    return arrow;
  }
}