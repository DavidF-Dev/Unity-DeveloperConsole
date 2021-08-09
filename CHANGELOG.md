# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
- Added documentation.
- Added buttons on the developer console to increase/decrease the log font size.
- Added helper method for logging a collection in list format.
- Changed command suggestions / autocomplete to also work with aliases.
- Fixed issues with displaying logs that exceed the rendering limit.
- Fixed issue with "customcommands" not showing commands created via the attribute method.

## [0.1.9-alpha] - 2021-08-05
- Added events for when the console is enabled/disabled, opened/closed and focused/unfocused.
- Added option for commands created using the attribute to only be available in development builds.
- Added helper methods to start coroutines or invoke methods after a time has passed.
- Added command: "log_size" - changes the font size used in the developer console log.
- Added command: "customcommands" - lists available custom commands.
- Changed key binds to be disabled when any object is selected by the current event system.
- Fixed issues to do with interacting with the input field.

## [0.1.7-alpha] - 2021-07-29
- Initial pre-release for internal testing.