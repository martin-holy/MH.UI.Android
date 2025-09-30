# Android UI Guidelines

# Layout Parameters Rule

Always use the layout parameters (`LayoutParams`) type defined by the **parent container** when adding child views.
Each `ViewGroup` subclass has its own `LayoutParams` type, which may extend `ViewGroup.LayoutParams` with additional properties (e.g., margins, weights).

* **Parent decides layout**: Child views don’t decide their own layout. The parent container interprets its own `LayoutParams` type.
* **Correct subclass**: Always provide the parent’s `LayoutParams` (e.g., `LinearLayout.LayoutParams` when inside a `LinearLayout`).
* **Shorthand in code**: Inside the context of a container class (e.g., `LinearLayout`), you can usually just write `LayoutParams`, and the IDE will resolve it to the correct subclass (`LinearLayout.LayoutParams`).
* **Be explicit if unclear**: If there is any ambiguity, explicitly specify the full type (`LinearLayout.LayoutParams`).
* **RecyclerView.ViewHolder**: Set RecyclerView.LayoutParams to ItemView. Default is WrapContent, WrapContent.

### Common LayoutParams Types

| Parent Container    | LayoutParams Subclass            | Extra Properties / Notes                     |
| ------------------- | -------------------------------- | -------------------------------------------- |
| `LinearLayout`      | `LinearLayout.LayoutParams`      | Supports `Weight`, `Gravity`                 |
| `RelativeLayout`    | `RelativeLayout.LayoutParams`    | Supports positioning rules (`AddRule`)       |
| `FrameLayout`       | `FrameLayout.LayoutParams`       | Supports `Gravity`                           |
| `ConstraintLayout`  | `ConstraintLayout.LayoutParams`  | Supports constraints (start, end, top, etc.) |
| `TableLayout`       | `TableLayout.LayoutParams`       | Supports `Column`, `Span`                    |
| `GridLayout`        | `GridLayout.LayoutParams`        | Supports row/column specs                    |
| `CoordinatorLayout` | `CoordinatorLayout.LayoutParams` | Supports behavior attachments                |
| Generic `ViewGroup` | `ViewGroup.LayoutParams`         | Only `Width` and `Height`                    |

### Example Usage

```csharp
var container = new LinearLayout(Context!);

// Correct: uses LinearLayout.LayoutParams (IDE may show just LayoutParams)
container.AddView(
  view,
  new LayoutParams(
    ViewGroup.LayoutParams.MatchParent,
    ViewGroup.LayoutParams.MatchParent));

// Explicit form (same as above, no ambiguity)
container.AddView(
  view,
  new LinearLayout.LayoutParams(
    ViewGroup.LayoutParams.MatchParent,
    ViewGroup.LayoutParams.MatchParent));
```


# Dispose Guidelines (Android + .NET Views)

These are the rules for handling `Dispose()` in Android UI with .NET.

---

## 1. General Rule
- Dispose **everything you create yourself** if it implements `IDisposable`.
- If the object was created by the framework and is owned/added into the **view hierarchy** (`AddView`), you normally **do not dispose it manually** — the parent will handle it.

---

## 2. Views
- ✅ If you **add a View using `AddView`**, do **not** call `Dispose()` on it manually.  
  The parent disposes children when it is disposed.
- ✅ If you **remove a View yourself** (`RemoveView`, `RemoveAllViews`), you should dispose that view (the root of the subtree).  
  The child hierarchy will be disposed recursively.
- ⚠️ Disposing a child still attached to a parent can cause crashes (`ObjectDisposedException`).

---

## 3. Events & Bindings
- Always **unsubscribe events** or use **weak references** for automatic cleanup.
- This avoids memory leaks where views or view models are kept alive longer than intended.

---

## 4. Drawables, Bitmaps, Streams
- If you **create them manually** (e.g. `new Bitmap`, `new Paint`, `new MemoryStream`), you must dispose them explicitly.
- If you obtain them from **resources** (`Context.GetDrawable`, `Resources.GetBitmap`), you do **not** dispose — Android manages these shared resources.

---

## 5. Collections (List, Dictionary, etc.)
- `List<T>`, `Dictionary<TKey, TValue>` and other standard collections do **not** implement `IDisposable`.  
  - Do **not** dispose.  
  - If they contain disposable objects you own, dispose those manually.  
  - Optionally call `.Clear()` if you want to release references earlier, but not required for GC.

---

## 6. RecyclerView and Adapters
- **RecyclerView itself**:
  - If added via `AddView`, it is disposed automatically with its parent.  
  - If you remove it yourself, dispose it explicitly.  

- **Adapter**:
  - Always dispose your custom `RecyclerView.Adapter` in the parent’s `Dispose`.  
  - To prevent leaks, **detach the adapter first**:
    ```csharp
    _recyclerView.SetAdapter(null); // break references
    _adapter.Dispose();
    ```
  - Do **not** dispose the adapter if it is still assigned to a live `RecyclerView`.

---

## 7. Summary Checklist
When implementing `Dispose(bool disposing)` in a custom view/control:
1. Unsubscribe from all events.
2. Dispose bindings, subscriptions, and adapters.
3. Dispose manually created disposables (bitmaps, streams, etc.).
4. If you removed any views, dispose them too.
5. Do not dispose children still managed by the parent hierarchy.

