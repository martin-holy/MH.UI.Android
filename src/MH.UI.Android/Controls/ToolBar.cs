using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls;

public class ToolBar : LinearLayout {
  private readonly Dictionary<string, Lazy<View>> _panels = [];
  private readonly Dictionary<Type, string[]> _typePanelsMap = [];

  public string[] DefaultPanelsKeys { get; set; } = [];

  public ToolBar(Context? context) : base(context) { }

  public void RegisterPanel(string key, Func<View> factory) {
    _panels[key] = new Lazy<View>(() => {
      var v = factory();
      AddView(v);
      return v;
    });
  }

  public void RegisterType(Type type, string[] panelKeys) {
    _typePanelsMap.Add(type, panelKeys);
  }

  public void Activate(Type? type) {
    _hide();
    if (type != null && _typePanelsMap.TryGetValue(type, out var keys))
      _show(keys);
    else
      _show(DefaultPanelsKeys);
  }

  private void _hide() {
    foreach (var panel in _panels.Values)
      if (panel.IsValueCreated)
        panel.Value.Visibility = ViewStates.Gone;
  }

  private void _show(string[] keys) {
    foreach (var key in keys)
      if (_panels.TryGetValue(key, out var panel))
        panel.Value.Visibility = ViewStates.Visible;
  }
}