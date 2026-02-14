using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;
using System;

namespace MH.UI.Android.Views;

public class LogItemV : LinearLayout, IBindable<LogItem> {
  private readonly View _level;
  private readonly TextView _text;

  public LogItemV(Context? context) : base(context) {
    SetBackgroundResource(Resource.Drawable.selectable_item);
    SetGravity(GravityFlags.CenterVertical);
    this.SetPadding(DimensU.Spacing);
    Orientation = Orientation.Horizontal;
    Clickable = true;
    Focusable = true;

    _level = new View(context);
    _text = new TextView(context);
    AddView(_level, new LayoutParams(DisplayU.DpToPx(10), LPU.Match));
    AddView(_text);
  }

  public void Bind(LogItem item) {
    _level.SetBackgroundResource(item.Level switch {
      LogLevel.Info => Resource.Color.c_log_info,
      LogLevel.Warning => Resource.Color.c_log_warning,
      LogLevel.Error => Resource.Color.c_log_error,
      _ => throw new ArgumentOutOfRangeException()
    });

    _text.Text = item.Title;
  }

  public void Unbind() { }
}