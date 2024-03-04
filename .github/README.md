# In-game Developer Console for Unity
This asset provides an <b>in-game developer console</b> (debug console) for Unity projects, allowing developers or players to execute commands or view incoming Unity messages (i.e. Debug.Log, errors, etc.)

The dev console window has a user-friendly look, inspired by Valve's Source engine console and Discord's user-interface. It includes text suggestion, autocomplete & key bindings that enable quick access to commands.

The asset is completely free, but if you'd like to show support you can [buy me a bowl of spaghetti](https://www.buymeacoffee.com/davidfdev) 🍝

<img src="/.github/preview2.gif" alt="Developer console preview" width="72%"></img>

## !!! Development notice !!!
**This asset is no longer in active development**, so use at your own risk.

If you are searching for an alternative, I can highly recommend [Quantum Console](https://www.qfsw.co.uk/docs/QC/articles/quickstart/quickstart.html). I have been using it in a few of my projects.

I am proud of my achievements developing this asset, but I unfortunately don't have the time to maintain or extend it. I may archive the repository at some point in the future.
Feel free to continue using the asset, or create a Fork of the source for your own needs, adhering to the license.

## Setup
Simply import the package into your project and you're good to go. No additional setup is required.
- Download directly from the [releases](https://github.com/DavidF-Dev/Unity-DeveloperConsole/releases) tab & import in Unity (<i>Assets>Import Package</i>).
- Import via the Unity package manager (<i>Window>Package Manager</i>).
  - Git URL: ``https://github.com/DavidF-Dev/Unity-DeveloperConsole.git``</br>
    <img src="/.github/install1.png" alt="Package manager install" width="25%"></src>
  - <i>Or</i> add the following line to <i>Packages/manifest.json</i>:</br>``"com.davidfdev.devconsole": "https://github.com/DavidF-Dev/Unity-DeveloperConsole.git"``

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
Default supported parameter types implement the [``IConvertible``](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible) interface (e.g. ``int``, ``float``, ``string``, ``bool``, etc.)</br>
To add a custom type, use ``DevConsole.AddParameterType<T>()`` (see FAQ below).</br>
- Enums are also supported.</br>
- Nullable-type parameters accept "null" and "~" (e.g. ``GameObject`` or ``Color?``).
- Value-type parameters accept "default" and "~" (e.g. ``Int32`` or ``Color``).

Commands that use a nullable bool (``Boolean?``) are used primarily as a toggle. For example, executing "<b>showfps ~</b>" will toggle showing the fps on-screen.

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
- ``commands``: display a list of available commands.
- ``help (String)commandName``: display helpful information about the specified command.
- ``print (String)message``: display a message in the dev console log.
- ``exit``: exit the game.
- ``fullscreen (Boolean?)enabled``: query or set whether the window is fullscreen.
- ``bind (InputKey)key (String)command``: bind the specified key to execute a command, even if the dev console window is closed.
- ``showfps (Boolean?)enabled``: query or set whether to display the fps on-screen.
- ``cs_evaluate (String)expression``: evaluate a C# expression or statement and display the result.
- ``scene_load (Int32)buildIndex``: load a scene.
- ``scene_info (Int32)sceneIndex``: display information about an active scene.
- ``obj_list``: display a hierarchical list of all game objects in the scene.
- ``obj_info (String)name``: display information about a game object.
- ``sys_info``: display information about the system.
- ``enum (String)enumName``: display all values of the specified enum.

And more...

## FAQ
<b>Q. Does the asset support the new input system?</b></br>
A. Yes, the dev console supports both the legacy and new input system. The correct input system will be chosen automatically by the asset during compilation.

<b>Q. Does the dev console work in release builds?</b></br>
A. Yes, the dev console can be used in release builds, but will need to be enabled via script: ``DevConsole.EnableConsole()``. It can be included in commercial projects as long as the [licensing conditions](https://github.com/DavidF-Dev/Unity-DeveloperConsole/blob/main/LICENSE.md) are fulfilled. Furthermore, specific commands can be set up to only work in development builds.

<b>Q. How do I add a custom parameter type?</b></br>
A. Use ``DevConsole.AddParameterType<T>()`` to enable the use of the specified type as parameters. A parser function must be provided, which converts a string into the parameter type.
```cs
DevConsole.AddParameterType<GameObject>((string input) => GameObject.Find(input));
```

<b>Q. Can I remove a built-in command?</b></br>
A. Yes, use ``DevConsole.RemoveCommand()`` to remove almost any command. There are a few permanent commands that cannot be removed that are deemed essential for the developer console to run correctly.

<b>Q. How can I stop keyboard input in the dev console from triggering game-specific actions (e.g. space will make the character jump even though the input field is focused)?</b></br>
A. As far as I know, this is an unavoidable issue. I recommend making a global property for your game (e.g. ``AllowGameInput``) which you can query before performing any game-specific input. This property can then reference ``DevConsole.IsOpenAndFocused``, effectively disabling game-specific input when the dev console is open and the input field is focused. 
```cs
public static bool AllowGameInput => !DevConsole.IsOpenAndFocused;

private void CheckGameInput()
{
  if (!AllowGameInput) return;
  
  // ... check game input
}
```

<b>Q. This isn't quite what I'm after</b></br>
A. There are alternatives available by other developers - each slightly different. If this one doesn't meet your needs, then maybe one of theirs will:
- [Quantum Console](https://www.qfsw.co.uk/docs/QC/articles/quickstart/quickstart.html)
- [yasirkula's Unity In-game Debug Console](https://github.com/yasirkula/UnityIngameDebugConsole).
- [popcron's Unity Console](https://github.com/popcron/console)
- [piveclabs' In-game Console](https://docs.piveclabs.com/assets-for-unity/developer-tools-for-unity/in-game-console)

## Acknowledgements
- [@exdli](https://twitter.com/exdli/status/14063908189448642653) for help with supporting both input systems.
- [SpeedCrunch](https://speedcrunch.org/) calculator which inspired the command suggestion / autocomplete design.
- [FiraCode](https://github.com/tonsky/FiraCode) font used under the SIL Open Font License 1.1.

## Contact
If you have any questions or would like to get in contact, shoot me an email at contact@davidfdev.com. Alternatively, you can send me a direct message on Twitter at [@DavidF_Dev](https://twitter.com/DavidF_Dev).</br></br>
Consider showing support by [buying me a bowl of spaghetti](https://www.buymeacoffee.com/davidfdev) 🍝</br>
View my other Unity tools on my [website](https://www.davidfdev.com/tools) 🔨
