# Contributing
Looking to contribute something? **Here's how you can help.**

**Please ask first before embarking on any significant pull request** (e.g., implementing features, refactoring code, porting to a different language), otherwise you risk spending a lot of time working on something that the project's developers might not want to merge into the project. _Open_ bugs that are _tagged_ as **Type: Bug** and _open_ tasks that are _tagged_ as both **Type: Task** and **~approved** are good places to start.

## Found a Bug?
A bug is a demonstrable problem that is caused by the code in the repository. If you find a bug, you can help us by submitting a bug report.

### Guidelines for bug reports:
1. **Use the GitHub issue search** — check if the bug has already been reported.
2. **Check if the issue has been fixed** — try to reproduce it using the latest code in the master branch in the repository.
3. **Isolate the problem** — ideally create an [SSCCE](http://www.sscce.org/) and a live example. Uploading the project on cloud storage (e.g., OneDrive) or creating a sample GitHub repository is also helpful.

Please try to be as detailed as possible in your report. A good bug report should include the following information:
*	What version of XAML Styler are you using? Are you using the plugin or standalone application?
*	What version of Visual Studio are you using?
*	What steps will reproduce the issue?
*	What would you expect to be the outcome?

## Feature requests
We love feature requests! If you have a great idea on how to improve XAML Styler, feel free to submit a feature request, but take a moment to decide whether your idea fits with the scope and aims of the project. It's up to you to make a strong case for your idea. Please provide as much detail and context as possible.

### Guidelines for feature requests:
1. **Use the GitHub issue search** — check if the feature request has already been made. Your idea might already have been submitted! Feel free to add to the discussion.
2. **Determine if the feature is a good fit** — ask yourself if the feature would benefit most users. Everyone has their own unique style, but we are more likely to accept features that have an impact on a larger number of users.
3. **Sell your idea** — a good feature request demonstrates a user need or pain point and how the proposed feature addresses it.

When in doubt, submit a feature request to start a discussion with the community to see if your idea is interesting enough to move forward with. 

## Pull requests
Pull requests from the community are awesome! If this is your first time contributing to this project, please check out our [setup instructions](#setup-instructions) and [coding guidelines](#coding-guidelines) before you begin. Pull requests should remain focused in scope, avoid containing unnecessary changes/commits, and pass all unit tests ([testing guidelines](#testing-guidelines)).

1. [Fork](http://help.github.com/fork-a-repo/) the project, clone your fork, and configure the remotes:
```
git clone https://github.com/Xavalon/XamlStyler.git  
cd <folder-name>  
git remote add upstream https://github.com/Xavalon/XamlStyler.git
```

2. Ensure you have the latest upstream changes:
```
git checkout master
git pull upstream master
```

3. Create a new feature branch off master for your change:
```
git checkout -b <feature-branch-name>
```

4. Commit your changes in logical chunks with clear and concise commit messages.

5. Locally merge (or rebase) the upstream development branch into your topic branch:
```
git pull [--rebase] upstream master
```

6. Ensure that your changes pass all unit tests.
7. Push your topic branch up to your fork:
```
git push origin <feature-branch-name>
```

8. [Open a Pull Request](https://help.github.com/articles/using-pull-requests/) with a clear title and description against the master branch.

## Setup Instructions
1. Ensure that you have the latest version of Visual Studio 2017 installed.
2. Check your installation to ensure you have the following optional workflows installed:
   * Visual Studio extension development (needed for primary extension)
   * .NET desktop development (needed for standalone command line application)
3. Develop awesome features!

## Testing Guidelines
1. Pull requests that contain bug fixes may or may not warrant adding additional unit tests, but they do need to pass all existing unit tests.
2. Pull requests that implement new features generally require adding additional unit tests and possibly updating existing unit tests. Please follow patterns and practices in the unit test project as best you can.

## Coding Guidelines
1. In general, we follow the [.NET Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/index) published by Microsoft.
2. If you are adding new options or modifying existing options, strings should be confirmed in the issue tracking the feature request.
3. Use the ```this``` keyword to qualify members in the current instance of a class.
4. If you add or change using directives, please ensure that you **Right Click -> Remove and Sort Usings** as necessary.
5. If you add files, please include ```// © Xavalon. All rights reserved.``` in the header.
