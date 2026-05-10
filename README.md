# :snowman: IceAge

![AI generated winterscape used for the splash screen](./src/Assets/LaunchImage.png)

IceAge is a [Mastodon](https://joinmastodon.org/) client written specifically (and only) for Windows using modern user interface paradigms. IceAge is currently in the early stages of development.

**Note on AI:** While the splash image is AI (and a placeholder for now), the rest of the code has been developed without the use of AI. This project is for me, for fun, a way for me to see how well my squishy brain, not silicon, can generate code.

### :dart: Goals

IceAge aims to be a first-class Mastodon client for the Windows platform. Unfortunately, Windows has few dedicated hobbyist developers interested in creating good open source software using the latest, modern UI frameworks, this includes any fist-class Mastodon clients. IceAge aims to rectify that. IceAge is written in C\# using modern .NET, and the [WinUI3](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/) framework.

- [X] Ability to login and authorize the application.
- [X] Ability to see a home timeline with streaming.
- [X] Media support.
- [ ] Settings page with settings support.
- [X] Timeline views for all timelines (home, local, federated) with streaming support.
- [ ] Media attachment support.
- [ ] Cards for supported URLs.
- [X] MVVM navigation.
- [ ] Opinionated WinUI interface with sensible customizations.
- [ ] Notifications.
- [ ] Ability to toot including with attachments.
- [ ] Batch file for building application.
- [ ] Poll Support
	- [X] Support for viewing polls.
	- [ ] Support for voting in polls.
	- [ ] Support for creating polls.

### :dart::dart: Stretch Goals

- [ ] Multiple account support.
- [ ] Heavy customization options.
- [ ] Multiple, responsive view options.
- [ ] Multi-column support.

## :hammer: Building & Requirements

I have only tested building on Windows 11 x64 using Visual Studio 2019. Support for CI/CD and simple `*.bat` based build system are planned in the future.

## :books: Library Dependencies

IceAge relies on the following FOSS libaries that have made the project much easier to develop!

- [Mastonet](https://github.com/glacasa/Mastonet) - Core library for native Mastodon API support for C\#/.NET
- [Blurhash.Core](https://github.com/MarkusPalcer/blurhash.net) - For creating palceholder while images load and bluring and content-warning images.
- [HtmlAgilityPack](https://html-agility-pack.net/) - To parse HTML toots as presented by the Mastodon API and render as native rich text.
- [Newtonsoft.Json](https://www.newtonsoft.com/json) - For creating and parsing JSON files used by IceAge.
