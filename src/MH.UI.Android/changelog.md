1.4.0:
	- [B] CollectionViewRowViewHolder: row height calculation when items are empty
	- [U] LoopPager: GetCurrentItem returns View
	- [N] LoopPager: PageChangedEvent
	- [N] ToolBar

1.3.0:
	- [N] ImagingU: ConvertOrientationFromMHToAndroid
	- [N] ExifInterfaceExtensions: SetLatLong, SetOrientation and SetUserComment
	- [N] BitmapExtensions: Create bitmap from file path and region
	- [N] BitmapExtensions: SaveAsJpeg
	- [N] BindingU: BindProgressBar
	- [N] BindingU: BindProgressText
	- [U] LoopPager: IsHorizontalGestureConsumer func for touch intercept
	- [N] ExifInterfaceExtensions: CopyAttributes
	- [N] ExifInterfaceExtensions: UpdateDimensions
	- [N] ImagingU: ResizeJpg
	- [U] FlatTreeItemViewHolderBase: optional icon and name
	- [N] TreeViewHostAdapter: support for custom TreeItemViewHolder
	- [N] ViewExtensions: WithPadding
	- [B] ImagingU: ApplyOrientation: switched angle
	- [U] LoopPager: support for nested ViewPager2
	- [N] TabControlHost: Slot for custom View in TabStrip
	- [N] TabControlHost: TabItemHeaderViewHolder text rotation
	- [B] CollectionViewItem: IsSelected binding
	- [U] LoopPager: AllowDragOnlyFromEdge
	- [U] CollectionViewHost: performance and item content creation
	- [U] CollectionViewHost: old version set as obsolete

1.2.0:
	- [N] IconToggleButton
	- [N] Icons: GetDrawable by iconName
	- [N] BindingU: IconToggleButton BindToggled
	- [U] LoopPager: Intercept touch if has horizontal scrollable under
	- [C] TabControlHost: using ItemMenuFactory from TabControl
	- [N] TextViewExtensions: WithTextColor
	- [B] BindingU: EditText BindText null reference when string is null
	- [U] DialogHost: Optionally full screen dialogs

1.1.0:
	- [N] BindingU: BindVisibility
	- [N] ZoomAndPanHost: ImageTransformUpdatedEvent
	- [N] Colors: c_transparent
	- [N] BackgroundFactory: Create with shadow

1.0.0:
	- initial release