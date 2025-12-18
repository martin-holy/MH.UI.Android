using Android.Content;
using Android.Content.Res;
using Android.Widget;
using AndroidX.Core.Content;
using MH.UI.Android.Utils;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls;

public class IconToggleButton : IconButton, ICheckable {
  private static readonly int[] _checkedStateSet = new int[] {
    global::Android.Resource.Attribute.StateChecked
  };

  private static readonly int[][] _tintStates = new int[][] {
    new[] {
      global::Android.Resource.Attribute.StateEnabled,
      global::Android.Resource.Attribute.StateChecked
    },
    new[] {
      global::Android.Resource.Attribute.StateEnabled,
      -global::Android.Resource.Attribute.StateChecked
    },
    new[] {
      -global::Android.Resource.Attribute.StateEnabled
    }
  };

  private bool _checked;

  public bool Checked {
    get => _checked;
    set {
      if (_checked == value) return;
      _checked = value;
      RefreshDrawableState();
      CheckedChanged?.Invoke(this, new(_checked));
    }
  }

  public event EventHandler<CompoundButton.CheckedChangeEventArgs>? CheckedChanged;

  public IconToggleButton(Context context, string iconName, Dictionary<object, object>? iconNameToColor = null) : base(context) {
    Clickable = true;
    Focusable = true;

    if (Icons.GetDrawable(context, iconName) is { } drawable) {
      _applyTint(drawable, Icons.GetColor(context, iconName, iconNameToColor ?? Icons.IconNameToColor));
      SetImageDrawable(drawable);
    }
  }

  public void Toggle() {
    Checked = !_checked;
  }

  public override bool PerformClick() {
    Toggle();
    return base.PerformClick();
  }

  public override int[]? OnCreateDrawableState(int extraSpace) {
    var drawableState = base.OnCreateDrawableState(extraSpace + 1);
    if (_checked)
      MergeDrawableStates(drawableState, _checkedStateSet);

    return drawableState;
  }

  private void _applyTint(global::Android.Graphics.Drawables.Drawable drawable, int checkedColor) {
    var colors = new int[] {
      checkedColor,
      ContextCompat.GetColor(Context, Resource.Color.c_white7),
      ContextCompat.GetColor(Context, Resource.Color.c_disabled_fo)
    };

    drawable.Mutate();
    drawable.SetTintList(new ColorStateList(_tintStates, colors));
  }
}