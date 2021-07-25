// File: DevConsole.cs
// Purpose: Provides a public interface for accessing the developer console
// Created by: DavidFDev

#if INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
#define USE_NEW_INPUT_SYSTEM
#endif

using System;
using UnityEngine;

using InputKey =
#if USE_NEW_INPUT_SYSTEM
    UnityEngine.InputSystem.Key;
#else
    UnityEngine.KeyCode;
#endif

namespace DavidFDev.DevConsole
{
    /// <summary>
    ///     Interface for accessing the developer console.
    /// </summary>
    public static class DevConsole
    {
        #region Static fields and constants

        private static DevConsoleMono _console;

        #endregion

        #region Static properties

        /// <summary>
        ///     Whether the dev console is enabled.
        /// </summary>
        public static bool IsEnabled
        {
            get => _console.ConsoleIsEnabled;
            set
            {
                if (value)
                {
                    EnableConsole();
                    return;
                }

                DisableConsole();
            }
        }

        /// <summary>
        ///     Whether the dev console window is open.
        /// </summary>
        public static bool IsOpen
        {
            get => _console.ConsoleIsShowing;
            set
            {
                if (value)
                {
                    _console.OpenConsole();
                    return;
                }

                _console.CloseConsole();
            }
        }

        /// <summary>
        ///     Whether the dev console window is open and the input field is focused.
        /// </summary>
        public static bool IsOpenAndFocused => _console.ConsoleIsShowingAndFocused;

        /// <summary>
        ///     Whether the dev console user-defined key bindings are enabled.
        /// </summary>
        public static bool IsKeyBindingsEnabled => _console.BindingsIsEnabled;

        /// <summary>
        ///     The key used to toggle the dev console window, NULL if no key.
        /// </summary>
        public static InputKey? ToggleKey
        {
            get => _console.ConsoleToggleKey;
            set => _console.SetToggleKey(value);
        }

        #endregion

        #region Static methods

        /// <summary>
        ///     Add a command to the dev console database.
        /// </summary>
        /// <param name="command">Use Command.Create() to define a command.</param>
        /// <param name="onlyInDevBuild">Whether to only add the command if the project is a development build.</param>
        /// <returns></returns>
        public static bool AddCommand(Command command, bool onlyInDevBuild = false)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            return _console.AddCommand(command, onlyInDevBuild);
        }

        /// <summary>
        ///     Remove a command from the dev console database.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool RemoveCommand(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return _console.RemoveCommand(name);
        }

        /// <summary>
        ///     Run a command using the provided input.
        /// </summary>
        /// <param name="input">Input as if it were typed directly into the dev console.</param>
        /// <returns></returns>
        public static bool RunCommand(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            return _console.RunCommand(input);
        }

        /// <summary>
        ///     Add a parameter type to the dev console database.
        ///     This will allow the provided type to be used as a parameter in commands.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parseFunc"></param>
        /// <returns></returns>
        public static bool AddParameterType<T>(Func<string, T> parseFunc)
        {
            if (parseFunc == null)
            {
                throw new ArgumentNullException(nameof(parseFunc));
            }

            return _console.AddParameterType(typeof(T), s => parseFunc(s));
        }

        /// <summary>
        ///     Log a message to the dev console.
        /// </summary>
        /// <param name="message"></param>
        public static void Log(object message)
        {
            _console.Log(message);
        }

        /// <summary>
        ///     Log a message to the dev console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="colour"></param>
        public static void Log(object message, Color colour)
        {
            _console.Log(message, ColorUtility.ToHtmlStringRGBA(colour));
        }

        /// <summary>
        ///     Log a variable to the dev console.
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        public static void LogVariable(string variableName, object value)
        {
            _console.LogVariable(variableName, value);
        }

        /// <summary>
        ///     Log an error message to the dev console.
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(object message)
        {
            _console.LogError(message);
        }

        /// <summary>
        ///     Log a warning message to the dev console.
        /// </summary>
        /// <param name="message"></param>
        public static void LogWarning(object message)
        {
            _console.LogWarning(message);
        }

        /// <summary>
        ///     Log a success message to the dev console.
        /// </summary>
        /// <param name="message"></param>
        public static void LogSuccess(object message)
        {
            _console.LogSuccess(message);
        }

        /// <summary>
        ///     Log a message with a seperator bar.
        /// </summary>
        /// <param name="message"></param>
        public static void LogSeperator(object message)
        {
            _console.LogSeperator(message);
        }

        /// <summary>
        ///     Log the most recently executed command syntax to the dev console.
        /// </summary>
        public static void LogCommand()
        {
            _console.LogCommand();
        }

        /// <summary>
        ///     Log command syntax to the dev console.
        /// </summary>
        /// <param name="name">Command name.</param>
        public static void LogCommand(string name)
        {
            _console.LogCommand(name);
        }

        /// <summary>
        ///     Set the key used to toggle the dev console window, NULL if no key.
        /// </summary>
        /// <param name="toggleKey"></param>
        public static void SetToggleKey(InputKey? toggleKey)
        {
            _console.SetToggleKey(toggleKey);
        }

        /// <summary>
        ///     Enable the dev console.
        /// </summary>
        public static void EnableConsole()
        {
            _console.EnableConsole();
        }

        /// <summary>
        ///     Disable the dev console, making it inaccessible.
        /// </summary>
        public static void DisableConsole()
        {
            _console.DisableConsole();
        }

        /// <summary>
        ///     Open the dev console window.
        /// </summary>
        public static void OpenConsole()
        {
            _console.OpenConsole();
        }

        /// <summary>
        ///     Close the dev console window.
        /// </summary>
        public static void CloseConsole()
        {
            _console.CloseConsole();
        }

        /// <summary>
        ///     Clear the contents of the dev console.
        /// </summary>
        public static void ClearConsole()
        {
            _console.ClearConsole();
        }

        /// <summary>
        ///     Register a callback for when the dev console is opened.
        /// </summary>
        /// <param name="callback"></param>
        public static void Register_OnDevConsoleOpened(Action callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _console.OnDevConsoleOpened += callback;
        }

        /// <summary>
        ///     Deregister a callback for when the dev console is opened.
        /// </summary>
        /// <param name="callback"></param>
        public static void Deregister_OnDevConsoleOpened(Action callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _console.OnDevConsoleOpened -= callback;
        }

        /// <summary>
        ///     Register a callback for when the dev console is closed.
        /// </summary>
        /// <param name="callback"></param>
        public static void Register_OnDevConsoleClosed(Action callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _console.OnDevConsoleClosed += callback;
        }

        /// <summary>
        ///     Deregister a callback for when the dev console is closed.
        /// </summary>
        /// <param name="callback"></param>
        public static void Deregister_OnDevConsoleClosed(Action callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _console.OnDevConsoleClosed -= callback;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#pragma warning disable IDE0051
        private static void Init()
#pragma warning restore IDE0051
        {
            _console = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/FAB_DevConsole.Instance")).GetComponent<DevConsoleMono>();
        }

        #endregion
    }
}