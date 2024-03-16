# IceAge

IceAge is a [Mastodon](https://joinmastodon.org/) client written specifically (and only) for Windows using modern user interface paradigms. IceAge is currently in the early stages of development.

### Goals

IceAge aims to be a first-class Mastodon client for the Windows platform. Unfortunately, Windows has few dedicated hobbyist developers interested in creating good open source software using the latest, modern UI frameworks, this includes any fist-class Mastodon clients. IceAge aims to rectify that. IceAge is written in C\# using modern .NET, and the [WinUI3](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/) framework.

-   Timeline views for all timelines (home, local, federated) with streaming support.
-   Media attachment support.
-   Cards for supported URLs.
-   MVVM navigation.
-   Opinionated WinUI interface with sensible customizations.
-   Notifications.
-   Ability to toot including with attachments.

### Stretch Goals

-   Multiple account support.
-   Heavy customization options.
-   Multiple, responsive view options.
-   Multi-column support.

### What is Working?

-   Ability to login and authorize the application.
-   Ability to see a home timeline with streaming.
-   Media support.
-   Settings page.

## Building & Requirements

I have only tested building on Windows 11 x64 using Visual Studio 2019. Support for CI/CD and simple `*.bat` based build system are planned in the future.
