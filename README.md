# In-game Developer Console for Unity
This asset provides an in-game developer console for Unity projects, allowing developers or users to execute commands or view incoming Unity messages (i.e. Debug.Log, errors, etc.).</br>
Created by DavidFDev.

## Setup
Simply import the package into your project and you're good to go. No additional setup is required.
- Import via the Unity asset store.
- Import via the Unity package manager.
- Download directly from the releases tab & import in Unity (<i>Assets > Import Package</i>).

## Usage
When the game is running, press tilde ~ to toggle the dev console window. The window has an input field along the bottom where commands can be entered. Pressing Enter will execute the typed command.

### Commands
Commands are in the form: <b>commandName parameter1 parameter2 ... parameterN</b>. Some commands have no parameters!

Typing "<b>commands</b>" will display a list of all available commands in the console log.</br>
Typing "<b>help print</b>" will provide helpful information about the <b>print</b> command.</br>
Typing "<b>print "Hello world!"</b>" will display the message "Hello world!" in the console log.

Text that is encased by quotation marks " will be interpreted as a single parameter.

### Scripting
The dev console can be accessed via the ``DevConsole`` static class in the ``DavidFDev`` namespace.
- ``Enable/DisableConsole()``: enable or disable the dev console entirely (disabled by default in release builds).
- ``Open/CloseConsole()``: open or close the dev console window.
- ``Log()``: log a message to the dev console.
- ``SetToggleKey()``: change or disable the key used to toggle the dev console window.
- ``AddCommand()``: add a custom command to the dev console database.

### Custom commands
Custom commands can be added to the dev console by developers. They can be created in two ways:
- Use ``Command.Create()`` to initialise a new command instance, allowing for multiple parameters and aliases.
- Add the ``[DevConsoleCommand]`` attribute above a static method declaration, using the method body as the callback and arguments as command parameters.

#### Parameters
Default supported parameter types implement the ``IConvertible`` interface (e.g. int, float, string, bool, etc.)</br>
Enums are also supported.</br>
To add a custom type, use ``DevConsole.AddParameterType()`` (advanced).

#### Example using Command.Create
```cs
DevConsole.AddCommand(Command.Create<string>(
  name: "print",
  aliases: "say,display",
  helpText: "Display a message in the dev console",
  p1: Parameter.Create(
    name: "message",
    helpText: "Message to display"
  ),
  callback: (string message) => DevConsole.Log(message)
));
```

#### Example using attribute
```cs
[DevConsoleCommand(
  name: "print",
  aliases: "say,display",
  helpText: "Display a message in the dev console",
  parameterHelpText: "Message to display"
)]
private static void Print(string message) => DevConsole.Log(message);
```

### Built-in commands
The asset provides various built-in commands.
- Example
