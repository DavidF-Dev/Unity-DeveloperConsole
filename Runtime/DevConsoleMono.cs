#define HIDE_FROM_USER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if NEW_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using InputKey =
#if NEW_INPUT_SYSTEM
    UnityEngine.InputSystem.Key;
#else
    UnityEngine.KeyCode;
#endif

namespace DavidFDev.DevConsole
{
#if HIDE_FROM_USER
    [AddComponentMenu("")]
#endif
    internal sealed class DevConsoleMono : MonoBehaviour
    {
        #region Static fields and constants

        private const string ErrorColour = "#E99497";
        private const string WarningColour = "#B3E283";
        private const string SuccessColour = "#B3E283";
        private const string ClearLogText = "Type <b>devconsole</b> for instructions on how to use the developer console.";
        private const float MinWidth = 650;
        private const float MaxWidth = 1200;
        private const float MinHeight = 200;
        private const float MaxHeight = 900;
        private const int CommandHistoryLength = 10;
        private const InputKey DefaultToggleKey =
#if NEW_INPUT_SYSTEM
            InputKey.Backquote;
#else
            InputKey.BackQuote;
#endif
        private const InputKey UpArrowKey = InputKey.UpArrow;
        private const InputKey DownArrowKey = InputKey.DownArrow;

        private static readonly Version Version = new Version(0, 1, 2);

        #endregion

        #region Fields

        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private Text _versionText = null;
        [SerializeField] private InputField _inputField = null;
        [SerializeField] private InputField _logField = null;
        [SerializeField] private RectTransform _logContentTransform = null;
        [SerializeField] private RectTransform _dynamicTransform = null;

        internal InputKey? consoleToggleKey = DefaultToggleKey;
        internal bool consoleIsEnabled = false;
        internal bool consoleIsShowing = false;
        private bool _init = false;
        private bool _rebuildLayout = false;
        private bool _focusInputField = false;
        private bool _repositioning = false;
        private Vector2 _repositionOffset = default;
        private bool _resizing = false;
        private Vector2 _initPosition = default;
        private Vector2 _initSize = default;
        private bool _displayUnityLogs = true;
        private bool _displayUnityErrors = true;
        private bool _displayUnityExceptions = true;
        private bool _displayUnityWarnings = true;
        private string _lastCommand = string.Empty;
        private readonly List<string> _commandHistory = new List<string>(CommandHistoryLength);
        private int _commandHistoryIndex = -1;
        private readonly Dictionary<string, Command> _commands = new Dictionary<string, Command>();

        #endregion

        #region Properties

        private string InputText
        {
            get => _inputField.text;
            set => _inputField.text = value;
        }

        private string LogText
        {
            get => _logField.text;
            set
            {
                _logField.text = value;
                _rebuildLayout = true;
            }
        }

        private int CaretPosition
        {
            get => _inputField.caretPosition;
            set => _inputField.caretPosition = value;
        }

        #endregion

        #region Events

        internal event Action OnDevConsoleOpened;

        internal event Action OnDevConsoleClosed;

        #endregion

        #region Methods

        internal void EnableConsole()
        {
            if (!_init && consoleIsEnabled)
            {
                return;
            }

            Application.logMessageReceived += OnLogMessageReceived;
            Application.logMessageReceivedThreaded += OnLogMessageReceived;
            consoleIsEnabled = true;
            enabled = true;
        }

        internal void DisableConsole()
        {
            if (!_init && !consoleIsEnabled)
            {
                return;
            }

            if (consoleIsShowing)
            {
                CloseConsole();
            }
            _dynamicTransform.anchoredPosition = _initPosition;
            _dynamicTransform.sizeDelta = _initSize;
            _commandHistory.Clear();
            Application.logMessageReceived -= OnLogMessageReceived;
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;
            consoleIsEnabled = false;
            enabled = false;
        }

        internal void OpenConsole()
        {
            if (!_init && (!consoleIsEnabled || consoleIsShowing))
            {
                return;
            }

            // Create a new event system if none exists
            if (EventSystem.current == null)
            {
                GameObject obj = new GameObject("EventSystem");
                EventSystem.current = obj.AddComponent<EventSystem>();
                obj.AddComponent<StandaloneInputModule>();
            }

            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            consoleIsShowing = true;
            _focusInputField = true;
            InputText = InputText.TrimEnd('`');

            OnDevConsoleOpened?.Invoke();
        }

        internal void CloseConsole()
        {
            if (!_init && (!consoleIsEnabled || !consoleIsShowing))
            {
                return;
            }

            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            consoleIsShowing = false;
            _repositioning = false;
            _resizing = false;

            OnDevConsoleClosed?.Invoke();
        }

        internal void ToggleConsole()
        {
            if (consoleIsShowing)
            {
                CloseConsole();
                return;
            }

            OpenConsole();
        }

        internal void ClearConsole()
        {
            LogText = ClearLogText;
        }

        internal void SubmitInput()
        {
            if (!string.IsNullOrWhiteSpace(InputText))
            {
                RunCommand(InputText.TrimEnd('\n'));
            }

            InputText = string.Empty;
        }

        internal bool RunCommand(string rawInput)
        {
            // Get the input as an array
            // First element is the command name
            // Remainder are raw parameters
            string[] input = GetInput(rawInput);

            // Find the command
            Command command = GetCommand(input[0]);

            // Add the input to the command history, even if it isn't a valid command
            AddToCommandHistory(input[0], rawInput);

            if (command == null)
            {
                LogError("Could not find the specified command: \"" + input[0] + "\".");
                return false;
            }

            // Determine the actual parameters now that we know the expected parameters
            input = ConvertInput(input, command.Parameters.Length);

            // Try to execute the default callback if the command has no parameters specified
            if (input.Length == 1 && command.DefaultCallback != null)
            {
                command.DefaultCallback();
                return true;
            }

            if (command.Parameters.Length != input.Length - 1)
            {
                LogError("Invalid number of parameters: " + command.ToFormattedString() + ".");
                return false;
            }

            // Iterate through the parameters and convert to the appropriate type
            object[] parameters = new object[command.Parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                string parameter = input[i + 1];

                try
                {
                    // Allow bools to be in the form of "0" and "1"
                    if (command.Parameters[i].Type == typeof(bool) && int.TryParse(parameter, out int result))
                    {
                        if (result == 0)
                        {
                            parameter = "false";
                        }
                        else if (result == 1)
                        {
                            parameter = "true";
                        }
                    }

                    // Try to convert the parameter input into the appropriate type
                    parameters[i] = Convert.ChangeType(parameter, command.Parameters[i].Type);
                }
                catch (Exception)
                {
                    LogError("Invalid parameter type: \"" + parameter + "\". Expected " + command.GetFormattedParameter(command.Parameters[i]) + ".");
                    return false;
                }
            }

            // Execute the command callback with the parameters, if any
            command.Callback(parameters.Length == 0 ? null : parameters);
            return true;
        }

        internal bool AddCommand(Command command, bool onlyInDevBuild = false)
        {
            if (onlyInDevBuild && !Debug.isDebugBuild)
            {
                return false;
            }

            // Try to fix the command name, removing any whitespace and converting it to lowercase
            command.FixName();

            // Try to add the command, making sure it doesn't conflict with any other commands
            if (!string.IsNullOrEmpty(command.Name) && !_commands.ContainsKey(command.Name) && !_commands.Values.Select(c => c.Aliases).Any(a => command.HasAlias(a)))
            {
                _commands.Add(command.Name, command);
                return true;
            }
            return false;
        }

        #region Log methods

        internal void Log(object message)
        {
            LogText += '\n' + message.ToString();
        }

        internal void Log(object message, string htmlColour)
        {
            Log("<color=" + htmlColour + ">" + message.ToString() + "</color>");
        }

        internal void LogVariable(string variableName, object value)
        {
            Log(variableName + ": " + value + ".");
        }

        internal void LogError(object message)
        {
            Log(message, ErrorColour);
        }

        internal void LogWarning(object message)
        {
            Log(message, WarningColour);
        }

        internal void LogSuccess(object message)
        {
            Log(message, SuccessColour);
        }

        internal void LogSeperator(object message = null)
        {
            if (message == null)
            {
                Log("---");
            }
            else
            {
                Log("--- <b>" + message.ToString() + "</b> ---");
            }
        }

        internal void LogCommand()
        {
            LogCommand(_lastCommand);
        }

        internal void LogCommand(string name)
        {
            Command command = GetCommand(name);
            if (command != null)
            {
                Log(">> " + command.ToFormattedString() + ".");
            }
        }

        #endregion

        #region Unity events

        internal void OnInputValueChanged()
        {
            // Submit the input if a new line is entered (ENTER)
            if (InputText.EndsWith("\n"))
            {
                SubmitInput();
            }
        }

        internal void OnRepositionButtonPointerDown(BaseEventData eventData)
        {
            _repositioning = true;
            _repositionOffset = ((PointerEventData)eventData).position - (Vector2)_dynamicTransform.position;
        }

        internal void OnRepositionButtonPointerUp(BaseEventData _)
        {
            _repositioning = false;
        }

        internal void OnResizeButtonPointerDown(BaseEventData _)
        {
            _resizing = true;
        }

        internal void OnResizeButtonPointerUp(BaseEventData _)
        {
            _resizing = false;
        }

        internal void OnAuthorButtonPressed()
        {
#if !UNITY_ANDROID && !UNITY_IOS
            Application.OpenURL(@"https://www.davidfdev.com");
#endif
        }

        #endregion

        #region Unity methods

        private void Awake()
        {
            _init = true;

            // Set up the game object
            gameObject.name = "DevConsoleInstance";
            DontDestroyOnLoad(gameObject);

#if HIDE_FROM_USER
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
#endif

            _versionText.text = "v" + Version.ToString();
            _initPosition = _dynamicTransform.anchoredPosition;
            _initSize = _dynamicTransform.sizeDelta;

            InitBuiltInCommands();
            InitAttributeCommands();

            // Enable the console by default if in editor or a development build
            if (Debug.isDebugBuild)
            {
                EnableConsole();
            }
            else
            {
                DisableConsole();
            }

            ClearConsole();
#if !HIDE_FROM_USER
            if (enabled)
            {
                OpenConsole();
            }
            else
            {
                CloseConsole();
            }
#else
            CloseConsole();
#endif

            _init = false;

#if NEW_INPUT_SYSTEM && UNITY_EDITOR
            // Check that the input system is in use (in editor)
            if (Keyboard.current == null)
            {
                Debug.LogWarning("Developer console has been disabled because the new Input System is in the project, but not enabled.");
                DisableConsole();
            }
#endif
        }

        private void Update()
        {
            if (!consoleIsEnabled || !consoleIsShowing)
            {
                return;
            }

            // Force the input field to be focused by the event system
            if (_focusInputField)
            {
                EventSystem.current.SetSelectedGameObject(_inputField.gameObject, null);
                _focusInputField = false;
            }

            // Move the developer console using the mouse position
            if (_repositioning)
            {
                Vector2 mousePosition = GetMousePosition();
                _dynamicTransform.position = new Vector3(
                    mousePosition.x - _repositionOffset.x,
                    mousePosition.y - _repositionOffset.y,
                    _dynamicTransform.position.z);
            }

            // Resize the developer console using the mouse position
            if (_resizing)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_dynamicTransform, GetMousePosition(), null, out Vector2 localPoint);
                localPoint.x = Mathf.Clamp(Mathf.Abs(localPoint.x), MinWidth, MaxWidth);
                localPoint.y = Mathf.Clamp(Mathf.Abs(localPoint.y), MinHeight, MaxHeight);
                _dynamicTransform.sizeDelta = localPoint;
            }
        }

        private void LateUpdate()
        {
            if (!consoleIsEnabled)
            {
                return;
            }

            // Force the canvas to rebuild layouts, which will display the log correctly
            if (_rebuildLayout)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_logContentTransform);
            }

            // Check if the developer console toggle key was pressed
            if (consoleToggleKey.HasValue && GetKeyDown(consoleToggleKey.Value))
            {
                ToggleConsole();
                return;
            }

            // Allow cycling through command history using the UP and DOWN arrows
            if (_inputField.isFocused)
            {
                if (_commandHistoryIndex != -1 && InputText.Length == 0)
                {
                    _commandHistoryIndex = -1;
                }

                if (GetKeyDown(UpArrowKey))
                {
                    CycleCommandHistory(1);
                }
                else if (GetKeyDown(DownArrowKey))
                {
                    CycleCommandHistory(-1);
                }
            }
        }

        #endregion

        private void InitBuiltInCommands()
        {
            #region Console commands

            AddCommand(Command.Create(
                "devconsole",
                "",
                "Display instructions on how to use the developer console",
                () =>
                {
                    LogSeperator("Developer console (v" + Version.ToString() + ")");
                    Log("Use <b>commands</b> to display a list of available commands.");
                    Log("Use " + GetCommand("help").ToFormattedString() + " to display information about a specific command.");
                    Log("Use UP / DOWN to cycle through command history.");
                    Log("");
                    Log("Created by @DavidFDev.");
                    LogSeperator();
                }
            ));

            AddCommand(Command.Create<string>(
                "print",
                "say",
                "Display a message in the developer console",
                Parameter.Create("message", "Message to display"),
                s => Log(s)
            ));

            AddCommand(Command.Create(
                "clear",
                "",
                "Clear the developer console",
                () => ClearConsole()
            ));

            AddCommand(Command.Create(
                "reset",
                "",
                "Reset the position and size of the developer console",
                () =>
                {
                    _dynamicTransform.anchoredPosition = _initPosition;
                    _dynamicTransform.sizeDelta = _initSize;
                    _rebuildLayout = true;
                }
            ));

            AddCommand(Command.Create<string>(
                "help",
                "info",
                "Display information about a specified command",
                Parameter.Create(
                    "commandName",
                    "Name of the command to get information about"),
                s =>
                {
                    Command command = GetCommand(s);

                    if (command == null)
                    {
                        LogError("Unknown command name specified: \"" + s + "\". Use <b>list</b> for a list of all commands.");
                        return;
                    }

                    LogSeperator(command.Name);

                    if (!string.IsNullOrEmpty(command.HelpText))
                    {
                        Log(command.HelpText + ".");
                    }

                    if (command.Aliases?.Length > 0 && command.Aliases.Any(a => !string.IsNullOrEmpty(a)))
                    {
                        string[] formattedAliases = command.Aliases.Select(alias => "<i>" + alias + "</i>").ToArray();
                        Log("Aliases: " + string.Join(", ", formattedAliases) + ".");
                    }

                    if (command.Parameters.Length > 0)
                    {
                        Log("Syntax: " + command.ToFormattedString());
                    }

                    foreach (Parameter parameter in command.Parameters)
                    {
                        if (!string.IsNullOrEmpty(parameter.HelpText))
                        {
                            Log(" <b>" + parameter.Name + "</b>: " + parameter.HelpText + ".");
                        }
                    }

                    LogSeperator();
                }
            ));

            AddCommand(Command.Create(
                "commands",
                "",
                "Display a sorted list of all available commands",
                () =>
                {
                    LogSeperator("Commands");
                    Log(string.Join(", ", _commands.Keys.OrderBy(s => s)));
                    LogSeperator();
                }
            ));

            AddCommand(Command.Create(
                "consoleversion",
                "",
                "Display the version of the developer console",
                () => Log("Developer console version: " + Version.ToString() + ".")
            ));

            #endregion

            #region Player commands

            AddCommand(Command.Create(
                "quit",
                "exit",
                "Exit the player application",
                () =>
                {
                    if (Application.isEditor)
                    {
                        LogError("Cannot quit the player application when running in the Editor.");
                        return;
                    }

                    Application.Quit();
                }
            ));

            AddCommand(Command.Create(
                "appversion",
                "",
                "Display the version of the application",
                () => Log("App version: " + Application.version + ".")
            ));

            AddCommand(Command.Create(
                "unityversion",
                "",
                "Display the version of the engine",
                () => Log("Engine version: " + Application.unityVersion + ".")
            ));

            AddCommand(Command.Create(
                "unityinput",
                "",
                "Display the Unity input system being used by the developer console",
                () =>
                {
#if NEW_INPUT_SYSTEM
                    Log("The new input system package is currently being used.");
#else
                    Log("The legacy input system is currently being used.");
#endif
                }
            ));

            #endregion

            #region Screen commands

            AddCommand(Command.Create<bool>(
                "fullscreen",
                "",
                "Query or set whether the window is fullscreen",
                Parameter.Create("enabled", "Whether the window is fullscreen"),
                b =>
                {
                    Screen.fullScreen = b;
                    Log((b ? "Enabled" : "Disabled") + " fullscreen mode.");
                },
                () =>
                {
                    LogVariable("Fullscreen", Screen.fullScreen);
                }
            ));

            #endregion

            #region Scene commands

            AddCommand(Command.Create<int>(
                "scene_load",
                "",
                "Load the scene at the specified build index",
                Parameter.Create(
                    "buildIndex",
                    "Build index of the scene to load, specified in the Unity build settings"
                    ),
                i =>
                {
                    if (i >= SceneManager.sceneCountInBuildSettings)
                    {
                        LogError("Invalid build index specified: \"" + i + "\". Check the Unity build settings.");
                        return;
                    }

                    SceneManager.LoadScene(i);
                    LogSuccess("Loaded scene at build index " + i + ".");
                }
            ), true);

            AddCommand(Command.Create<int>(
                "scene_info",
                "",
                "Display information about the current scene",
                Parameter.Create("sceneIndex", "Index of the scene in the currently loaded scenes"),
                i =>
                {
                    if (i >= SceneManager.sceneCount)
                    {
                        LogError("Could not find active scene at index: " + i + ".");
                        return;
                    }

                    Scene scene = SceneManager.GetSceneAt(i);
                    LogSeperator(scene.name);
                    Log("Scene index: " + i + ".");
                    Log("Build index: " + scene.buildIndex + ".");
                    Log("Path: " + scene.path + ".");
                    LogSeperator();
                },
                () =>
                {
                    if (SceneManager.sceneCount == 0)
                    {
                        Log("Could not find any active scenes.");
                        return;
                    }

                    LogSeperator("Active scenes");
                    for (int i = 0; i < SceneManager.sceneCount; i++)
                    {
                        Scene scene = SceneManager.GetSceneAt(i);
                        Log(i + ") " + scene.name + ", build index: " + scene.buildIndex + ".");
                    }
                    LogCommand();
                    LogSeperator();
                }
            ));

            #endregion

            #region Log commands

            AddCommand(Command.Create<bool>(
                "log_logs",
                "",
                "Query, enable or disable displaying Unity logs in the developer console",
                Parameter.Create("enabled", "Whether Unity logs should be displayed in the developer console"),
                b =>
                {
                    _displayUnityLogs = b;
                    LogSuccess((b ? "Enabled" : "Disabled") + " displaying Unity logs in the developer console.");
                },
                () =>
                {
                    LogVariable("Log unity logs", _displayUnityLogs);
                }
            ));

            AddCommand(Command.Create<bool>(
                "log_errors",
                "",
                "Query, enable or disable displaying Unity errors in the developer console",
                Parameter.Create("enabled", "Whether Unity errors should be displayed in the developer console"),
                b =>
                {
                    _displayUnityErrors = b;
                    LogSuccess((b ? "Enabled" : "Disabled") + " displaying Unity errors in the developer console.");
                },
                () =>
                {
                    LogVariable("Log unity errors", _displayUnityErrors);
                }
            ));

            AddCommand(Command.Create<bool>(
                "log_exceptions",
                "",
                "Query, enable or disable displaying Unity exceptions in the developer console",
                Parameter.Create("enabled", "Whether Unity exceptions should be displayed in the developer console"),
                b =>
                {
                    _displayUnityExceptions = b;
                    LogSuccess((b ? "Enabled" : "Disabled") + " displaying Unity exceptions in the developer console.");
                },
                () =>
                {
                    LogVariable("Log unity exceptions", _displayUnityExceptions);
                }
            ));

            AddCommand(Command.Create<bool>(
                "log_warnings",
                "",
                "Query, enable or disable displaying Unity warnings in the developer console",
                Parameter.Create("enabled", "Whether Unity warnings should be displayed in the developer console"),
                b =>
                {
                    _displayUnityWarnings = b;
                    LogSuccess((b ? "Enabled" : "Disabled") + " displaying Unity warnings in the developer console.");
                },
                () =>
                {
                    LogVariable("Log unity warnings", _displayUnityWarnings);
                }
            ));

            #endregion
        }

        private void InitAttributeCommands()
        {
            // https://github.com/yasirkula/UnityIngameDebugConsole/blob/master/Plugins/IngameDebugConsole/Scripts/DebugLogConsole.cs
            // Implementation of finding attributes sourced from yasirkula's code

#if UNITY_EDITOR || !NETFX_CORE
            string[] ignoredAssemblies = new string[]
            {
                "Unity",
                "System",
                "Mono.",
                "mscorlib",
                "netstandard",
                "TextMeshPro",
                "Microsoft.GeneratedCode",
                "I18N",
                "Boo.",
                "UnityScript.",
                "ICSharpCode.",
                "ExCSS.Unity",
#if UNITY_EDITOR
				"Assembly-CSharp-Editor",
                "Assembly-UnityScript-Editor",
                "nunit.",
                "SyntaxTree.",
                "AssetStoreTools"
#endif
            };
#endif

#if UNITY_EDITOR || !NETFX_CORE
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
#else
            foreach (Assembly assembly in new Assembly[] { typeof(DebugConsoleMono).Assembly })
#endif
            {
#if (NET_4_6 || NET_STANDARD_2_0) && (UNITY_EDITOR || !NETFX_CORE)
                if (assembly.IsDynamic)
                    continue;
#endif

                string assemblyName = assembly.GetName().Name;

#if UNITY_EDITOR || !NETFX_CORE
                if (ignoredAssemblies.Any(a => assemblyName.ToLower().StartsWith(a.ToLower())))
                {
                    continue;
                }
#endif

                try
                {
                    foreach (Type type in assembly.GetExportedTypes())
                    {
                        foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                        {
                            foreach (object attribute in method.GetCustomAttributes(typeof(DebugConsoleCommandAttribute), false))
                            {
                                DebugConsoleCommandAttribute commandAttribute = (DebugConsoleCommandAttribute)attribute;
                                if (commandAttribute != null)
                                {
                                    AddCommand(Command.Create(commandAttribute, method));
                                }
                            }
                        }
                    }
                }
                catch (NotSupportedException) { }
                catch (System.IO.FileNotFoundException) { }
                catch (Exception e)
                {
                    Debug.LogError("Error whilst searching for debug console command attributes in assembly(" + assemblyName + "): " + e.Message + ".");
                }
            }
        }

        private void OnLogMessageReceived(string logString, string stackTrace, LogType type)
        {
            string time = DateTime.Now.ToString("HH:mm:ss tt");
            switch (type)
            {
                case LogType.Log:
                    if (!_displayUnityLogs)
                    {
                        return;
                    }
                    Log("(" + time + ") <b>Log:</b> " + logString);
                    break;
                case LogType.Error:
                    if (!_displayUnityErrors)
                    {
                        return;
                    }
                    Log("(" + time + ") <color=" + ErrorColour + "><b>Error:</b> </color>" + logString);
                    break;
                case LogType.Exception:
                    if (!_displayUnityExceptions)
                    {
                        return;
                    }
                    Log("(" + time + ") <color=" + ErrorColour + "><b>Exception:</b> </color>" + logString);
                    break;
                case LogType.Warning:
                    if (!_displayUnityWarnings)
                    {
                        return;
                    }
                    Log("(" + time + ") <color=" + WarningColour + "><b>Warning:</b> </color>" + logString);
                    break;
                default:
                    break;
            }
        }

        private Command GetCommand(string name)
        {
            return _commands.TryGetValue(name.ToLower(), out Command command) ? command : _commands.Values.FirstOrDefault(c => c.HasAlias(name));
        }

        private string[] GetInput(string rawInput)
        {
            string[] split = rawInput.Split(' ');
            if (split.Length <= 1)
            {
                return split;
            }

            List<string> parameters = new List<string>()
            {
                split[0]
            };
            bool buildingParameter = false;
            string parameter = "";
            for (int i = 1; i < split.Length; i++)
            {
                if (!buildingParameter)
                {
                    if (split[i].StartsWith("\"") && i != split.Length - 1)
                    {
                        if (!split[i].EndsWith("\""))
                        {
                            buildingParameter = true;
                            parameter = split[i].TrimStart('"');
                        }
                        else
                        {
                            parameters.Add(split[i].Trim('"'));
                        }
                    }
                    else
                    {
                        parameters.Add(split[i]);
                    }
                }
                else
                {
                    if (split[i].EndsWith("\"") || i == split.Length - 1)
                    {
                        buildingParameter = false;
                        parameter += " " + split[i].TrimEnd('\"');
                        parameters.Add(parameter);
                    }
                    else
                    {
                        parameter += " " + split[i];
                    }
                }
            }

            return parameters.ToArray();
        }

        private string[] ConvertInput(string[] input, int parameterCount)
        {
            if (input.Length - 1 <= parameterCount)
            {
                return input;
            }

            string[] newInput = new string[parameterCount + 1];
            newInput[0] = input[0];
            string aggregatedFinalParameter = "";
            for (int i = 1; i < input.Length; i++)
            {
                if (i - 1 < parameterCount - 1)
                {
                    newInput[i] = input[i];
                }
                else if (i - 1 == parameterCount - 1)
                {
                    aggregatedFinalParameter = input[i];
                }
                else
                {
                    aggregatedFinalParameter += " " + input[i];
                }
            }
            newInput[newInput.Length - 1] = aggregatedFinalParameter;
            return newInput;
        }

        private void AddToCommandHistory(string name, string input)
        {
            _lastCommand = name;
            _commandHistory.Insert(0, input);
            if (_commandHistory.Count == CommandHistoryLength)
            {
                _commandHistory.RemoveAt(_commandHistory.Count - 1);
            }
            _commandHistoryIndex = -1;
        }

        private void CycleCommandHistory(int direction)
        {
            if (_commandHistory.Count == 0 ||
                (_commandHistoryIndex == _commandHistory.Count - 1 && direction == 1) ||
                (_commandHistoryIndex == -1 && direction == -1))
            {
                return;
            }

            if (_commandHistoryIndex == 0 && direction == -1)
            {
                _commandHistoryIndex = -1;
                InputText = string.Empty;
                return;
            }

            _commandHistoryIndex += direction;
            InputText = _commandHistory[_commandHistoryIndex];
            CaretPosition = InputText.Length;
        }

        #region Input methods

        private bool GetKeyDown(InputKey key)
        {
#if NEW_INPUT_SYSTEM
            if (Keyboard.current == null)
            {
                return false;
            }

            return Keyboard.current[key].wasPressedThisFrame;
#else
            return Input.GetKeyDown(key);
#endif
        }

        private Vector2 GetMousePosition()
        {
#if NEW_INPUT_SYSTEM
            return Mouse.current.position.ReadValue();
#else
            return Input.mousePosition;
#endif
        }

        #endregion

        #endregion
    }
}