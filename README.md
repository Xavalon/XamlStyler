XamlStyler
==========
A fork of the original Xaml Styler plugin as found on http://xamlstyler.codeplex.com
This fork has been created to continue development on the awesome XAML styler plugin created by Chris Chaochen. To support the efforts Chris has put into this project my fork will only support Visual Studio 2013 and higher. For Visual Studio 2012 support please install Chris his version.

Features
==========
* Format/Beautify Xaml markup in one click. 

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
<pre><span>&lt;</span><span>TextBlock</span><span>&gt;</span><br>  <span>&lt;</span><span>Run</span><span>&gt;</span>A<span>&lt;</span><span>Run</span><span>&gt;</span><br>  <span>&lt;</span><span>Run</span><span>&gt;</span>B<span>&lt;</span><span>Run</span><span>&gt;</span><br><span>&lt;/</span><span>TextBlock</span><span>&gt;</span><br></pre>
</div>
</td>
<td>&nbsp;vs</td>
<td>
<div>
<pre><span>&lt;</span><span>TextBlock</span><span>&gt;</span><br>  <span>&lt;</span><span>Run</span><span>&gt;</span>A<span>&lt;</span><span>Run</span><span>&gt;</span><span>&lt;</span><span>Run</span><span>&gt;</span>B<span>&lt;</span><span>Run</span><span>&gt;</span><br><span>&lt;/</span><span>TextBlock</span><span>&gt;</span><br></pre>
</div>
</td>
</tr>
</tbody>
</table>

* Indent Xaml markup based on "Tab Size/Indent Size/Indent Charater" settings available in "Option/Text Editor/XAML/Tabs" page. 

<img src="http://i60.tinypic.com/106x5pi.jpg" alt="markup settings" />

* XamlStyler specific options. 
  * Define your own attribute ordering rules 
  * Define your own attribute line break rules 
  * Markup extension formatting 
  * Automatically reformat Xaml file on saving 
  
<img src="http://i62.tinypic.com/11tbpqp.jpg" alt="xamlstyler options" /> 
  
* Import/Export XamlStyler settings. 

<img src="http://i59.tinypic.com/o8doon.jpg" alt="export settings" />

Contribute
==========
* Download the Visual Studio 2013 SDK from http://www.microsoft.com/en-us/download/details.aspx?id=40758
* Fork the XamlStyler project into your own GitHub account
* Develop some awesome features
* Create a pull request when ready
* Wait for us to merge your request

Thanks to our contributers!
===========================
* Bart Lannoeye
* Kevin Dockx
* Philip Hoppe
* Pedro Lamas