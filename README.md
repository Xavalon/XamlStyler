XAML Styler
==========
A fork of the original XAML Styler plugin as found on http://xamlstyler.codeplex.com
This fork has been created to continue development on the awesome XAML styler plugin created by Chris Chaochen. To support the efforts Chris has put into this project my fork will only support Visual Studio 2013 and higher. For Visual Studio 2012 support please install Chris his version.

####Join the discussion####
[![Join the chat at https://gitter.im/Xavalon/XamlStyler](https://badges.gitter.im/Xavalon/XamlStyler.svg)](https://gitter.im/Xavalon/XamlStyler?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) 

Features
==========
* Format/Beautify XAML markup in one click. 

<img src="http://i59.tinypic.com/fehok3.jpg" alt="beautify option" />

* Sort attributes based on following rules:
  * x:Class 
  * XML Namespaces 
    * WPF built-in namespaces 
    *  User defined namespaces 
  * Key, Name or Title attributes 
    * x:Key 
    * x:Name 
    * Title 
  * Grid or Canvas related attached layout attributes 
    * Numeric layout attributes Width/MinWidth/MaxWidth 
    * Height/MinHeight/MaxHeight 
    * Margin 
  * Alignment related attributes HorizontalAlignment/ContentHorizontalAlignment 
    * VerticalAlignment/ContentVerticalAlignment 
    * Panel.ZIndex 
  * Other attributes 
* Short attributes tolerance. 
  * When an element contains 2 or less than 2 attributes, line break is not applied for * better readability. 
* Special characters(e.g., &) are preserved. 
* Respect "significant" whitespace situation. 
  * No new linefeed will be added to <Run/>, if it is immediatly following *another element to prevent the rendering of unexpected space. 

<table>
<tbody>
<tr>
<th width="350">Significant Whitespace between &lt;Run/&gt;<br>
</th>
<th>&nbsp;</th>
<th width="350">No Whitespace between &lt;Run/&gt;</th>
</tr>
<tr>
<td><img src="http://xamlstyler.codeplex.com/download?DownloadId=156790" alt="" width="125" height="72"></td>
<td>&nbsp;vs</td>
<td><img src="http://xamlstyler.codeplex.com/download?DownloadId=156789" alt="" width="84" height="78"></td>
</tr>
<tr>
<td>
<div>
<pre><span>&lt;</span><span>TextBlock</span><span>&gt;</span><br>  <span>&lt;</span><span>Run</span><span>&gt;</span>A<span>&lt;/</span><span>Run</span><span>&gt;</span><br>  <span>&lt;</span><span>Run</span><span>&gt;</span>B<span>&lt;/</span><span>Run</span><span>&gt;</span><br><span>&lt;/</span><span>TextBlock</span><span>&gt;</span><br></pre>
</div>
</td>
<td>&nbsp;vs</td>
<td>
<div>
<pre><span>&lt;</span><span>TextBlock</span><span>&gt;</span><br>  <span>&lt;</span><span>Run</span><span>&gt;</span>A<span>&lt;/</span><span>Run</span><span>&gt;</span><span>&lt;</span><span>Run</span><span>&gt;</span>B<span>&lt;/</span><span>Run</span><span>&gt;</span><br><span>&lt;/</span><span>TextBlock</span><span>&gt;</span><br></pre>
</div>
</td>
</tr>
</tbody>
</table>

* Indent XAML markup based on "Tab Size/Indent Size/Indent Charater" settings available in "Option/Text Editor/XAML/Tabs" page. 

<img src="http://i60.tinypic.com/106x5pi.jpg" alt="markup settings" />

* XAML Styler specific options. 
  * Define your own attribute ordering rules 
  * Define your own attribute line break rules 
  * Markup extension formatting 
  * Automatically reformat XAML file on saving 
  
<img src="http://i62.tinypic.com/11tbpqp.jpg" alt="xamlstyler options" /> 
  
* Import/Export XAML Styler settings. 

<img src="http://i59.tinypic.com/o8doon.jpg" alt="export settings" />

* External Configurations

A valid XAML Styler configuration is a JSON file with one or more configuration options specified. Below is a complete external configuration that contains the default values. This path can be a local or network location.

Default Configuration:
```json
{
    "AttributesTolerance": 2,
    "KeepFirstAttributeOnSameLine": true,
    "MaxAttributeCharatersPerLine": 0,
    "MaxAttributesPerLine": 1,
    "NewlineExemptionElements": "RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransfom, SkewTransform, RotateTransform, TranslateTransform, Trigger, Condition, Setter",
    "SeparateByGroups": false,
    "AttributeIndentation": 0,
    "AttributeIndentationStyle": 1,
    "RemoveDesignTimeReferences":  false,
    "EnableAttributeReordering": true,
    "AttributeOrderingRuleGroups": [
        "x:Class",
        "xmlns, xmlns:x",
        "xmlns:*",
        "Key, x:Key, Uid, x:Uid",
        "Name, x:Name, Title",
        "Grid.Row, Grid.RowSpan, Grid.Column, Grid.ColumnSpan, Canvas.Left, Canvas.Top, Canvas.Right, Canvas.Bottom",
        "Width, Height, MinWidth, MinHeight, MaxWidth, MaxHeight, Margin",
        "HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment, Panel.ZIndex",
        "*:*, *",
        "PageSource, PageIndex, Offset, Color, TargetName, Property, Value, StartPoint, EndPoint",
        "mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText"
    ],
    "FirstLineAttributes": "",
    "OrderAttributesByName": true,
    "PutEndingBracketOnNewLine": false,
    "RemoveEndingTagOfEmptyElement": true,
    "SpaceBeforeClosingSlash": true,
    "RootElementLineBreakRule": 0,
    "ReorderVSM": 2,
    "ReorderGridChildren": true,
    "ReorderCanvasChildren": true,
    "ReorderSetters": 0,
    "FormatMarkupExtension": true,
    "NoNewLineMarkupExtensions": "x:Bind",
    "ThicknessSeparator": 0,
    "ThicknessAttributes": "Margin, Padding, BorderThickness, ThumbnailClipMargin",
    "FormatOnSave": true,
    "CommentPadding": 2,
    "IndentSize": 2
}
```

Thanks to our contributers!
===========================
* Bart Lannoeye
* Kevin Dockx
* Philip Hoppe
* Pedro Lamas
* RandomEngy
