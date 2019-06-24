using Mono.Addins;
using Mono.Addins.Description;

[assembly: Addin("XamlStyler", Namespace = "Xavalon", Version = "1.1.2")]

[assembly: AddinName("XAML Styler")]
[assembly: AddinCategory("IDE extensions")]
[assembly: AddinDescription("XAML Styler is a visual studio extension that formats XAML source code based on a set of styling rules. This tool can help you/your team maintain a better XAML coding style as well as a much better XAML readability.")]
[assembly: AddinAuthor("Xavalon; Ben Lacey; Taras Shevchuk")]
[assembly: AddinUrl("https://github.com/Xavalon/XamlStyler/")]

[assembly: AddinDependency("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
[assembly: AddinDependency("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]
[assembly: AddinDependency("::MonoDevelop.SourceEditor2", MonoDevelop.BuildInfo.Version)]