###XAML Styler###
XAML Styler is a visual studio extension, which formats XAML source code by sorting the attributes based on their importance. This tool can help you/your team maintain a better XAML coding style as well as a much better XAML readability.

|[Download from VS Gallery](https://visualstudiogallery.msdn.microsoft.com/3de2a3c6-def5-42c4-924d-cc13a29ff5b7)|[Documentation](https://github.com/Xavalon/XamlStyler/wiki)|[Script Integration](https://github.com/Xavalon/XamlStyler/wiki/Script-Integration)|[Change Log](https://github.com/Xavalon/XamlStyler/wiki/Change-Log)|[Contributing](https://github.com/Xavalon/XamlStyler/blob/master/CONTRIBUTING.md)|
|---|---|---|---|---|

[![Join the chat at https://gitter.im/Xavalon/XamlStyler](https://badges.gitter.im/Xavalon/XamlStyler.svg)](https://gitter.im/Xavalon/XamlStyler?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) 

<sub>XAML Styler is a fork of Chris Chaochen's [XAML Styler](http://xamlstyler.codeplex.com)<sub>

==========

####Features####
==========
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

* Import/Export XAML Styler settings. 

<img src="http://i59.tinypic.com/o8doon.jpg" alt="export settings" />

Thanks to our contributers!
===========================
* Bart Lannoeye
* Kevin Dockx
* Philip Hoppe
* Pedro Lamas
* RandomEngy
