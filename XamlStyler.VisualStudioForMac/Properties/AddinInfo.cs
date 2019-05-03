using Mono.Addins;
using Mono.Addins.Description;

[assembly: Addin("XamlStyler", Namespace = "Xavalon", Version = "1.1.1")]

[assembly: AddinName("XamlStyler")]
[assembly: AddinCategory("IDE extensions")]
[assembly: AddinDescription("A Visual Studio for Mac port of the XamlStyler Visual Studio extension.\nReformat your XAML to be more readable, maintainable and beautiful)")]
[assembly: AddinAuthor("Xavalon; Ben Lacey; Taras Shevchuk")]
[assembly: AddinUrl("https://github.com/Saratsin/XamlStyler/")]

[assembly: AddinDependency("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
[assembly: AddinDependency("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]
[assembly: AddinDependency("::MonoDevelop.SourceEditor2", MonoDevelop.BuildInfo.Version)]