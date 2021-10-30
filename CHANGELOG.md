# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
- Added user-defined developer console stats which are displayed on-screen.
- Changed preferences file to be deleted when unable to load correctly.
- Fixed exception not being caught when an issue occurs serialising or deserialising the preferences file.
- Fixed exception not being caught when reading deserialising an object from the preferences file.

## [1.0.0] - 2021-08-22
- Initial major release.
- Added LogException methods.
- Added DisableToggleKey method.
- Changed the size of the input field so that the text is larger.
- Changed enum command parameter to support namespaces to distinguish between enums with same names.
- Changed resolution command names to be easier to understand.
- Fixed strings being displayed incorrectly by cs_evaluate.
- Fixed issue with cached enum types.
- Fixed input field not unfocusing when the developer console window is closed.

## [0.2.2-alpha] - 2021-08-20
- Added real-time parameter information when typing out a command.
- Added built-in support for null and default values for parameters.
- Added support for CTRL + Backspace in the input field.
- Added commands for executing C# expressions or statements at runtime.
- Added exception handling for command callbacks.
- Added a reminder in the devconsole command that the console is disabled by default in release builds.
- Added locks to DevConsole.Log methods to ensure nothing scary happens when using multiple threads.
- Added Color as a supported parameter type.
- Added command: "colour" - displays the provided colour in the developer console.
- Changed DevConsole.InvokeCoroutine method to return the Coroutine instance.
- Changed some method access levels from internal to public.
- Fixed Unity logs from other threads not showing in the developer console log.
- Fixed generic or nullable parameter types not having their type displayed nicely.
- Fixed log scroll view not snapping to bottom intuitively.

## [0.2.1-alpha] - 2021-08-09
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