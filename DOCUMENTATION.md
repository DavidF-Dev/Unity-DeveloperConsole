# Documentation
It is recommended to view this file in a markdown viewer.
View on [GitHub](https://github.com/DavidF-Dev/Unity-DeveloperConsole/blob/main/DOCUMENTATION.md). 

## Usage
When the game is running, press tilde ``~`` to toggle the dev console window. The window has an input field along the bottom where commands can be entered. Pressing ENTER will execute the typed command.
- Use the UP / DOWN arrows to cycle through the command history or suggested commands.
- Use TAB to autocomplete a suggested command.

### Commands
Commands are in the form: <b>commandName parameter1 parameter2 ... parameterN</b>. Some commands have no parameters!

Typing "<b>commands</b>" will display a list of all available commands in the console log.</br>
Typing "<b>help print</b>" will provide helpful information about the <b>print</b> command.</br>
Typing "<b>print "Hello world!"</b>" will display the message "Hello world!" in the console log.

Text that is encased by quotation marks ``"`` will be interpreted as a single parameter.

### Scripting
The dev console can be accessed via the ``DevConsole`` static class in the ``DavidFDev.DevConsole`` namespace.
- ``Enable/DisableConsole()``: enable or disable the dev console entirely (disabled by default in release builds).
- ``Open/CloseConsole()``: open or close the dev console window.
- ``Log()``: log a message to the dev console.
- ``SetToggleKey()``: change or disable the key used to toggle the dev console window.
- ``AddCommand()``: add a custom command to the dev console database.

#### Example
```cs
using DavidFDev.DevConsole;
DevConsole.EnableConsole();
DevConsole.SetToggleKey(null);
DevConsole.Log("Hello world!");
```

### Custom commands
Custom commands can be added to the dev console by developers. They can be created in two ways:
- Use ``Command.Create()`` to initialise a new command instance, allowing for multiple parameters and aliases.
- Add the ``[DevConsoleCommand]`` attribute above a static method declaration, using the method body as the callback and arguments as command parameters.

#### Parameters
Default supported parameter types implement the [``IConvertible``](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible) interface (e.g. int, float, string, bool, etc.)</br>
Enums are also supported.</br>
Commands that use a nullable bool (``Boolean?``) parameter accept "~", "!", "null", and "toggle" - used primarily as a toggle.</br>
For example, executing "<b>showfps !</b>" will toggle showing the fps on-screen.</br></br>
To add a custom type, use ``DevConsole.AddParameterType<T>()`` (see FAQ below).

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

## Built-in commands
Listed below are all the built-in commands that come with the developer console by default.

### Console commands
- ``devconsole``: Display instructions on how to use the developer console.
- ``print (String)message``: Display a message in the developer console.
- ``clear``: Clear the developer console.
- ``reset``: Reset the position and size of the developer console.
- ``closeconsole``: Close the developer console window.
- ``help (String)commandName``: Display information about a specified command.
- ``enum (String)enumName``: Display information about a specified enum.
- ``commands``: Display a sorted list of all available commands.
- ``consoleversion``: Display the developer console version.
- ``bind``: Add a key binding for a command.
- ``unbind``: Remove a key binding.
- ``binds``: List all the key bindings.
- ``customcommands``: List all available custom commands.
- ``log_size``: Query or set the font size used in the developer console log.

### Player commands
- ``quit``: Exit the player application.
- ``appversion``: Display the application version.
- ``unityversion``: Display the unity version.
- ``unityinput``: Display the input system being used by the developer console.
- ``path``: Display the path to the application executable.
- ``showfps (Boolean)enabled``: Query or set whether the fps is being displayed on-screen.

### Screen commands
- ``fullscreen (Boolean)enabled``: Query or set whether the window is full screen.
- ``fullscreen_mode (FullScreenMode)mode``: Query or set the full screen mode.
- ``vsync (Int32)vSyncCount``: Query or set whether VSync is enabled.
- ``resolution``: Display the current screen resolution.
- ``targetfps (Int32)targetFrameRate``: Query or set the target frame rate.

### Camera commands
- ``cam_ortho (Boolean)enabled``: Query or set whether the main camera is orthographic.
- ``cam_fov (Int32)fieldOfView``: Query or set the main camera field of view.

### Scene commands
- ``scene_load (Int32)buildIndex``: Load the scene at the specified build index.
- ``scene_info (Int32)sceneIndex``: Display information about the current scene.
- ``obj_info (String)name``: Display information about a game object in the scene.
- ``obj_list``: Display a hierarchical list of all game objects in the scene.

### Log commands
- ``log_logs (Boolean)enabled``: Query, enable or disable displaying Unity logs in the developer console.
- ``log_errors (Boolean)enabled``: Query, enable or disable displaying Unity errors in the developer console.
- ``log_exceptions (Boolean)enabled``: Query, enable or disable displaying Unity exceptions in the developer console.
- ``log_warnings (Boolean)enabled``: Query, enable or disable displaying Unity warnings in the developer console.

### Misc commands
- ``time``: Display the current time.
- ``sys_info``: Display system information.
- ``datapath``: Display information about where data is stored by Unity and the developer console.