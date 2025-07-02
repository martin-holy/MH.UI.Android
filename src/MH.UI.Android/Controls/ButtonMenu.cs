using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MH.UI.Android.Controls;

public class ButtonMenu : LinearLayout {
  private ImageButton _menuButton = null!;
  private PopupWindow? _rootMenu;
  private MenuItem? _root;

  public MenuItem? Root {
    get => _root;
    set {
      _root = value;
      if (_root == null) return;
      _menuButton.SetImageDrawable(Icons.GetIcon(Context, _root.Icon));
      _rootMenu = CreateMenu(Context!, _menuButton, _root);
    }
  }

  public ButtonMenu(Context context) : base(context) => _initialize(context);
  public ButtonMenu(Context context, IAttributeSet attrs) : base(context, attrs) => _initialize(context);
  protected ButtonMenu(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) => _initialize(Context!);

  private void _initialize(Context context) {
    Orientation = Orientation.Horizontal;

    _menuButton = new ImageButton(context) {
      LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
    };
    _menuButton.Click += (_, _) => _rootMenu?.ShowAsDropDown(_menuButton);
    AddView(_menuButton);
  }

  public static PopupWindow CreateMenu(Context context, View parent, MenuItem root) {
    var listView = new ListView(context) {
      LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent),
      ScrollBarStyle = ScrollbarStyles.OutsideOverlay,
      Divider = null,
      DividerHeight = 0,
      Adapter = new ButtonMenuAdapter(context, [.. root.Items.Cast<MenuItem>()], parent)
    };
    listView.SetBackgroundResource(Resource.Drawable.view_border);
    listView.SetPadding(DisplayU.GetDP(1), DisplayU.GetDP(1), DisplayU.GetDP(1), DisplayU.GetDP(1));

    // Measure ListView with screen constraints
    int maxWidth = DisplayU.Metrics.WidthPixels;
    int maxHeight = (int)(DisplayU.Metrics.HeightPixels * 0.6); // Cap at 60% screen
    listView.Measure(
      MeasureSpec.MakeMeasureSpec(maxWidth, MeasureSpecMode.AtMost),
      MeasureSpec.MakeMeasureSpec(maxHeight, MeasureSpecMode.AtMost)
    );
    int width = Math.Max(listView.MeasuredWidth, DisplayU.GetDP(120)); // Min width 120dp
    int height = Math.Min(listView.MeasuredHeight, maxHeight);

    return new PopupWindow(listView, width, height, true);
  }
}

public class ButtonMenuAdapter(Context context, List<MenuItem> items, View parent) :
  ArrayAdapter<MenuItem>(context, Resource.Layout.menu_item, items) {

  private readonly List<MenuItem> _items = items;
  private readonly View _parent = parent;

  public override View GetView(int position, View? convertView, ViewGroup parent) {
    var item = _items[position];
    var view = new MenuItemHost(Context).Bind(item);
    view.Click += _onMenuItemClick;

    return view;
  }

  private void _onMenuItemClick(object? sender, EventArgs e) {
    if (sender is not MenuItemHost { DataContext: { } item } host) return;

    if (item.Items.Count == 0) {
      if (item.Command?.CanExecute(null) == true)
        item.Command.Execute(null);

      return;
    }

    var subPopup = ButtonMenu.CreateMenu(Context!, _parent, item);

    // Position submenu
    int[] location = new int[2];
    host.GetLocationOnScreen(location);
    int x = location[0] + host.Width; // Right of parent item
    int y = location[1];

    // Adjust if off-screen
    var displayMetrics = Context!.Resources!.DisplayMetrics!;
    int maxX = displayMetrics.WidthPixels - subPopup.Width;
    int maxY = displayMetrics.HeightPixels - subPopup.Height;
    x = Math.Min(x, maxX);
    y = Math.Min(y, maxY);

    // Avoid overlap with parent
    if (x == location[0] + host.Width && x + subPopup.Width > location[0]) {
      x = location[0] - subPopup.Width; // Show to left if overlap
    }

    subPopup.ShowAtLocation(_parent, GravityFlags.NoGravity, x, y);
  }
}