using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
    "XamlStyler",
    Namespace = "Xavalon",
    Version = "1.0.2"
)]

[assembly:AddinName ("XamlStyler")]
[assembly:AddinCategory ("IDE extensions")]
[assembly:AddinDescription ("A Xamarin Studio port of the Visual Studio XamlStyler extension.  Reformat your XAML to be more readable, maintainable and beautiful!")]
[assembly:AddinAuthor ("Xavalon & Ben Lacey")]

[assembly:AddinDependency ("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
[assembly: AddinDependency("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]
[assembly: AddinDependency("::MonoDevelop.SourceEditor2", MonoDevelop.BuildInfo.Version)]
