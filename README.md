# Brainf\*ck#
A complete, full-featured Brainf\*ck IDE/console for Windows 10

<a href="https://www.microsoft.com/store/apps/9nblgggzhvq5?cid=github"><img src="https://developer.microsoft.com/en-us/store/badges/images/English_get-it-from-MS.png" alt="Get it from Microsoft" width='280' /></a>

## Overview

![image](https://user-images.githubusercontent.com/10199417/92245082-1ade5300-eec4-11ea-96c3-faa56a9d3546.png)

* IDE with syntax highlight and code autocompletion
* Indentation depth indicators and column guides
* Git diff markers to see the changes in the current code file
* Breakpoints support with complete debugging experience
* Full featured console to test your scripts and improve your coding level
* Real time memory state with the all the info about every cell and the pointer actual position
* Exceptions handler, infinite loops auto suppression
* Complete 8 bit Unicode char lookup table to see the values of the characters in the Stdin buffer
* Touch friendly UI with on screen custom keyboard and dedicated text navigation buttons
* Undo/redo features to help you write your code more easily
* An interpreter debugger that shows the position of the operator that caused the exception and the stack trace
* Save your scripts in your personal library shared across devices and edit them whenever you want
* Custom overflow mode for the memory state
* PBrain extensions support to declare and call functions

## Required dependencies 🔧

The **Brainf\*ck#** solution requires the following tools to build all the available projects:

- [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/), with .NET and UWP workloads
- [Windows 10 SDK, version 1903](https://developer.microsoft.com/en-us/windows/downloads/sdk-archive)
- [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/visual-studio-sdks)
- Windows 10 >= 1903

## Contributing 🙌

**Brainf\*ck#** is open source and free to download in the Store, with just an _optional_ IAP to unlock additional themes for the IDE. If you found a bug, feel free to open an issue or draft a PR! If you'd like to support me instead, you can:

 - Give a star to the repository
 - Download the app and leave a review in the Store
 - Make a small contribution on my [PayPal.me link](https://www.paypal.me/sergiopedri) 🍻

## Dependencies and references 🔖

- [Windows Community Toolkit](https://github.com/windows-toolkit/WindowsCommunityToolkit)
- [WinUI](https://github.com/Microsoft/microsoft-ui-xaml)
- [Win2D](https://github.com/microsoft/Win2D)
- [refit](https://github.com/reactiveui/refit)
- [Stubble](https://github.com/StubbleOrg/Stubble)
- [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet)

## Trivia 📖

This project was the one I used to prototype and develop both the [`Microsoft.Toolkit.Mvvm`](https://www.nuget.org/packages/Microsoft.Toolkit.Mvvm/) and [`Microsoft.Toolkit.HighPerformance`](https://www.nuget.org/packages/Microsoft.Toolkit.HighPerformance/) packages, which are heavily used in the projects in this repository.

If you'd like to learn more on these libraries:

- See our samples and docs on the MVVM Toolkit at [aka.ms/mvvmtoolkit](https://aka.ms/mvvmtoolkit)
- See the docs on the HighPerformance package [here](https://docs.microsoft.com/windows/communitytoolkit/high-performance/introduction)
- Explore the codebase for these and other Windows Community Toolkit packages [here](https://aka.ms/wct)
