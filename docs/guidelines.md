# Android UI Guidelines

## Layout Parameters Rule

Always use the layout parameters (`LayoutParams`) type defined by the **parent container** when adding child views. Each `ViewGroup` subclass has its own `LayoutParams` type, which may extend `ViewGroup.LayoutParams` with additional properties (e.g., margins, weights).

* **Parent decides layout**: Child views don’t decide their own layout. The parent container interprets its own `LayoutParams` type.
* **Correct subclass**: Always provide the parent’s `LayoutParams` (e.g., `LinearLayout.LayoutParams` when inside a `LinearLayout`).
* **Shorthand in code**: Inside the context of a container class (e.g., `LinearLayout`), you can usually just write `LayoutParams`, and the IDE will resolve it to the correct subclass (`LinearLayout.LayoutParams`).
* **Be explicit if unclear**: If there is any ambiguity, explicitly specify the full type (`LinearLayout.LayoutParams`).

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

✅ Always match `LayoutParams` type to the parent container.
⚠️ Don’t use `ViewGroup.LayoutParams` unless you’re working with a very generic `ViewGroup` and no parent-specific subclass is available.
⚠️ Don’t assign LayoutParams in RecyclerView.ViewHolder root views — RecyclerView will enforce MATCH_PARENT width and WRAP_CONTENT height automatically.
