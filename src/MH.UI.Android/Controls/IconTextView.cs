using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;

namespace MH.UI.Android.Controls;

public class IconTextView : LinearLayout {
  private readonly IconView _icon;
  private readonly TextView _text;
  private readonly ColorStateList? _defaultTextColors;
  private readonly int _textPadding;

  public IconTextView(Context context) : base(context) {
    Orientation = global::Android.Widget.Orientation.Horizontal;
    SetGravity(GravityFlags.CenterVertical);

    _icon = new IconView(context);
    _text = new TextView(context);

    AddView(_icon, new LayoutParams(DimensU.IconSize, DimensU.IconSize)
      .WithMargin(DimensU.Spacing, 0, DimensU.Spacing, 0));
    AddView(_text);

    _defaultTextColors = _text.TextColors;
    _textPadding = _text.PaddingLeft;
  }

  public IconTextView(Context context, string? iconName, string text) : this(context) {
    BindIcon(iconName);
    BindText(text);
  }

  public IconTextView BindIcon(string? iconName, int? colorId = null) {
    if (!_icon.SetVisibleIf(!string.IsNullOrEmpty(iconName))) return this;
    _icon.Bind(iconName!, colorId);
    return this;
  }

  public IconTextView BindIcon(int? drawableId, int? colorId = null) {
    if (!_icon.SetVisibleIf(drawableId != null)) return this;
    _icon.Bind((int)drawableId!, colorId);
    return this;
  }

  public IconTextView BindText(string text, int? colorId = null) {
    if (!_text.SetVisibleIf(!string.IsNullOrEmpty(text))) return this;

    _text.Text = text;
    _text.SetPadding(_icon.Visibility == ViewStates.Visible ? 0 : _textPadding, _textPadding, _textPadding, _textPadding);

    if (colorId != null)
      _text.SetTextColor(new Color(ContextCompat.GetColor(Context, (int)colorId)));
    else
      _text.SetTextColor(_defaultTextColors);

    return this;
  }
}