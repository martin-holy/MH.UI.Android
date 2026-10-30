# MH.UI.Android

**MH.UI.Android** is the native Android implementation and platform integration layer for [MH.UI](https://github.com/martin-holy/MH.UI). It connects platform-independent controls, dialogs, and interaction logic to Android's native view system and provides reusable Android controls, binding support, and platform utilities.

The library is built with .NET for Android and uses native Android views and AndroidX components, including RecyclerView and ViewPager2.

## Highlights

- **Native hosts for shared controls** — Android implementations of `CollectionView`, `TreeView`, `TabControl`, `TreeMenu`, `DialogHost`, `ZoomAndPan`, and slide-panel layouts from `MH.UI`.
- **Virtualized collection and tree views** — RecyclerView-based presentation with reusable item views, incremental updates, selection, and support for custom item content.
- **MVVM binding integration** — Binding helpers for Android views, commands, properties, and shared `BindingScope` lifetimes.
- **Media playback** — Android media-player implementation connected to the shared media-player logic, with playback controls and video surfaces.
- **Zoomable media views** — Image and video views with zoom, pan, viewport handling, asynchronous preview loading, and full-resolution image loading.
- **Navigation and paging** — Looping pager with nested gesture handling, tab hosting, hierarchical menus, and slide-panel layouts.
- **Native dialogs** — Android presentations for shared message, input, selection, grouping, progress, and toggle dialogs.
- **Reusable Android controls** — Icon controls, sliders, toolbars, selectable item views, wrapping layouts, and other UI components.
- **Platform utilities** — Bitmap and EXIF helpers, MediaStore integration, display and dimension utilities, clipboard access, and Android view extensions.

## Hosts for MH.UI controls

These components connect the platform-independent models and behavior from `MH.UI` to native Android views.

- `CollectionViewHost` — RecyclerView-based host for `MH.UI`'s `CollectionView`, including item recycling, custom item content, view modes, grouping, and multi-selection.
- `TreeViewHost` — Virtualized hierarchical view host with expandable items, selection, scrolling, and support for custom item view holders.
- `TreeMenuHost` — Presents hierarchical menu data, including nested items and separators.
- `TabControlHost` — Hosts shared tab-control logic with selectable tab headers, tab content, and optional item menus.
- `DialogHost` — Android `DialogFragment` integration for the shared dialog workflows.
- `ZoomAndPanHost` — Connects the shared zoom-and-pan interaction model to Android touch and viewport handling.
- `SlidePanelHost` / `SlidePanelsGridHost` — Present shared slide panels in individual and grid-based layouts.

## Controls

### Collections and selection

- `ItemsView<T>` — RecyclerView-based view for displaying collections with reusable, bindable item views.
- `SelectableItemsView<T>` — Collection view with item selection and a corresponding adapter.
- `IconItemsView` — Displays icon-based items in a horizontal layout.
- `IconWrappedItemsView` — Displays icon-based items in a wrapping layout.
- `SelectionBar` — Displays the current tree selection and provides selection-clearing interaction.
- `ListItemV` — Android view for presenting shared `IListItem` models.
- `LogV` / `LogItemV` — Views for displaying and interacting with log entries.

### Media and image presentation

- `AndroidMediaPlayer` — Implements `IUiMediaPlayer` using Android's native `MediaPlayer`.
- `MediaPlayerControlPanel` — Playback controls for the shared media-player model.
- `VideoSurface` — Texture-based surface for video rendering.
- `LazyVideoView` — Video view that separates preview presentation from playback.
- `ZoomableImageView` — Image view with zoom and pan, asynchronous preview and full-image providers, and viewport-aware rendering.
- `ZoomableVideoView` — Video presentation with preview mode, playback, and zoom-and-pan interaction.
- `ViewportMatrixBuilder` — Builds image and texture transformation matrices for viewport rendering.

### Navigation and layout

- `LoopPager` — Looping page container with touch interception and gesture coordination for nested horizontally scrollable views.
- `WrapLayout` — Layout container that arranges child views into multiple rows as available width changes.
- `ToolBar` — Configurable toolbar that activates registered panels based on the current content type.
- `Slider` — Slider control based on Android `SeekBar`.
- `PopupSlider` — Slider with popup-based value interaction.
- `ButtonMenu` — Icon button with an associated menu.
- `IconButton`, `IconToggleButton`, `IconView`, and `IconTextView` — Reusable icon and icon-and-text controls.
- `CompactIconTextButton` — Compact button combining an icon and text.

## Dialogs

The `Dialogs` namespace contains native Android view presentations for the shared dialog models:

- `MessageDialogV` — Message and confirmation dialogs.
- `InputDialogV` — Text input dialogs.
- `SelectFromListDialogV` — Selectable list dialogs.
- `GroupByDialogV` — Grouping configuration dialogs.
- `ProgressDialogV` — Progress presentation.
- `ToggleDialogV` — Toggle and option dialogs.

`DialogHost` manages their presentation through Android fragments and connects dialog results to the shared `MH.UI` dialog logic.

## Binding and MVVM integration

The `Binding` namespace integrates Android views with the binding infrastructure from `MH.Utils`.

- **BindingExtensions** — Binding helpers for common Android controls and view properties, including text, visibility, progress, selection, and slider values.
- **CommandBinding** — Connects Android view interactions to `ICommand`, including command parameters, enabled state, and optional command text and icons.
- **BindingScope integration** — Groups bindings and subscriptions so they can be disposed together when a view or presentation is no longer active.

## Android utilities and extensions

- **MediaStoreU** — Retrieves image and video thumbnails, deletes media files, shares files, and handles media scanning through Android's MediaStore APIs.
- **ImagingU** — Android imaging helpers, including image resizing and orientation handling.
- **BitmapExtensions** — Bitmap creation, resizing, conversion, and JPEG saving helpers.
- **ExifInterfaceExtensions** — Helpers for reading and writing EXIF attributes through Android's `ExifInterface`.
- **IconU** — Resolves library icons and Android drawable resources.
- **DimensU / DisplayU** — Display metrics, density conversion, and common UI dimensions.
- **LayoutU / LPU** — Helpers for creating Android layouts and layout parameters.
- **ClipboardU** — Clipboard integration.
- **ViewExtensions / ViewGroupExtensions / TextViewExtensions / ImageViewExtensions / WindowExtensions** — Common Android view, text, image, layout, and window operations.
- **BackgroundFactory** — Creates reusable drawable backgrounds.
- **ErrorMessages** — Android-specific error message helpers.

## Dependencies and platform integration

MH.UI.Android builds on [MH.UI](https://github.com/martin-holy/MH.UI) for platform-independent controls, dialogs, and interaction logic, and [MH.Utils](https://github.com/martin-holy/MH.Utils) for MVVM, bindings, collections, and shared utilities.

It uses native .NET for Android APIs and AndroidX RecyclerView and ViewPager2. Android-specific hosts implement the interfaces exposed by `MH.UI`, while native views provide rendering, input handling, and lifecycle integration.

## Installation

MH.UI.Android is available as the `MH.UI.Android` NuGet package.

- NuGet: [MH.UI.Android](https://www.nuget.org/packages/MH.UI.Android)

For an application using the library, see [PictureManager.Android](https://github.com/martin-holy/PictureManager.Android).
