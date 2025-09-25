using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using MH.UI.Android.Utils;

namespace MH.UI.Android.Controls;

public class CompactIconTextButton : FrameLayout {
  public IconView Icon { get; }
  public TextView Text { get; }

  public CompactIconTextButton(Context context) : base(context) {
    Background = ContextCompat.GetDrawable(context, Resource.Drawable.image_button_background);
    
    Icon = new IconView(context);
    Icon.SetPadding(DimensU.Spacing, DimensU.Spacing, DimensU.Spacing * 2, DimensU.Spacing * 2);
    Text = new TextView(context);
    Text.SetPadding(0, 0, DisplayU.DpToPx(2), 0);

    AddView(Icon, new LayoutParams(DimensU.IconButtonSize, DimensU.IconButtonSize) { Gravity = GravityFlags.Center });
    AddView(Text, new LayoutParams(LPU.Wrap, LPU.Wrap) { Gravity = GravityFlags.Right | GravityFlags.Bottom });
  }
}