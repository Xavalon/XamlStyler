# Add `NewLineForCommaDelimitedValueAttributes` option

Closes #541, relates to #512

## What this does

Adds a new **Attribute Formatting** option: **New line for comma-delimited value attributes** (`NewLineForCommaDelimitedValueAttributes`). It takes a comma-separated **list of attribute names**. For any attribute whose name is in the list, each comma-separated segment of its value is placed on its own line, aligned under the first value character.

Default is empty (feature off). Add `Selector` to enable it for Avalonia style selectors.

---

## Why a list of attribute names (not a global boolean)

An earlier iteration applied to *every* plain-string attribute containing a comma. That is unsafe: XML normalizes whitespace inside attribute values, so wrapping a value onto a new line with indentation changes the value itself.

```
Text="Hello, world"   →   Text="Hello,
                                 world"
```

After XML attribute-value normalization, the second form's runtime value becomes `Hello,               world` (the newline + indentation collapse into literal spaces). The same breakage hits format strings such as `Text="{}{0:#,##0.00}"`.

Comma-in-value is common in text, tooltips, and format strings, and there is no reliable way to tell "a list I can wrap" from "prose I must not touch" by inspecting the value. Making the option an explicit **allow-list of attribute names** means only attributes the user knows are whitespace-insensitive (e.g. Avalonia `Selector`) are ever wrapped. This also matches the existing option style in the codebase (`NewlineExemptionElements`, `ThicknessAttributes`, `FirstLineAttributes`, `NoNewLineMarkupExtensions`).

---

## Examples

Config: `"NewLineForCommaDelimitedValueAttributes": "Selector"`

### Avalonia style selectors — split & aligned

**Before:**
```xml
<Style Selector="ContentControl Button, ContentControl StackPanel, ContentControl TextBlock">
```

**After:**
```xml
<Style Selector="ContentControl Button,
                 ContentControl StackPanel,
                 ContentControl TextBlock">
```

This works even when the element is under the attribute tolerance (a `<Style>` with only a `Selector` attribute still wraps).

### Non-listed attributes are never touched

Because only `Selector` is listed, everything else keeps its commas inline — no data corruption:

```xml
<TextBlock Text="Hello, world"
           ToolTip.Tip="{Binding Path=Name, UpdateSourceTrigger=PropertyChanged}" />
<Border Margin="1,2,3,4" Padding="8,4,8,4" />
```

Markup extensions remain governed solely by the existing `FormatMarkupExtension` option; this feature never affects them.

---

## Configuration

```json
{
  "NewLineForCommaDelimitedValueAttributes": "Selector"
}
```

Add more names (comma-separated) for any other whitespace-insensitive attributes you use.

---

## Changes

| File | Change |
|------|--------|
| `IStylerOptions.cs` | Added `NewLineForCommaDelimitedValueAttributes` (string list) in Attribute Formatting |
| `StylerOptions.cs` | Added property with JSON serialization, description, `DefaultValue("")` |
| `AttributeInfoFormatter.cs` | Added `ToCommaDelimitedMultiLineString()` — splits on commas, aligns continuation lines |
| `ElementDocumentProcessor.cs` | Splits only listed attributes; forces multi-line when a listed attribute has commas even under the attribute tolerance |
| `FileHandlingIntegrationTests.cs` | Integration test (covers listed split, under-tolerance split, and non-listed attributes staying inline) |
| `AttributeInfoFormatterUnitTests.cs` | Unit tests for `ToCommaDelimitedMultiLineString` |
| `TestFiles/TestNewLineForCommaDelimitedAttributeValues.*` | Test input and expected output |
| `TestConfigurations/AllDifferent.json` | Added option to round-trip serialization test |
| `XamlStyler.UnitTest.csproj` | Registered new test files for `CopyToOutputDirectory` |
