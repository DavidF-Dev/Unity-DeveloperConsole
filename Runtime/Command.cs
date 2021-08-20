// File: Command.cs
// Purpose: Statically allows the creation of a command instance and internally holds the data
// Created by: DavidFDev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DavidFDev.DevConsole
{
    /// <summary>
    ///     Command information used by the developer console.
    /// </summary>
    public sealed class Command
    {
        #region Static methods

        /// <summary>
        ///     Create a new command with no parameters.
        /// </summary>
        /// <param name="name">Name used to call the command (e.g. "quit").</param>
        /// <param name="aliases">Optional names that can be used to call the command, seperated by commas (e.g. "exit,shutdown").</param>
        /// <param name="helpText">Description of the command (e.g. "Quit the game").</param>
        /// <param name="callback">Callback to invoke when the command is called.</param>
        /// <returns></returns>
        public static Command Create(string name, string aliases, string helpText, Action callback)
        {
            return new Command()
            {
                Name = name,
                Aliases = aliases.Split(','),
                HelpText = helpText,
                Parameters = new Parameter[0],
                Callback = _ => callback()
            };
        }

        /// <inheritdoc cref="Create{T1, T2, T3, T4, T5}(string, string, string, Parameter, Parameter, Parameter, Parameter, Parameter, Action{T1, T2, T3, T4, T5}, Action)" />
        public static Command Create<T1>(string name, string aliases, string helpText, Parameter p1, Action<T1> callback, Action defaultCallback = null)
        {
            return new Command()
            {
                Name = name,
                Aliases = aliases.Split(','),
                HelpText = helpText,
                Parameters = new Parameter[] { p1.SetType<T1>() },
                Callback = o => callback((T1)o[0]),
                DefaultCallback = defaultCallback
            };
        }

        /// <inheritdoc cref="Create{T1, T2, T3, T4, T5}(string, string, string, Parameter, Parameter, Parameter, Parameter, Parameter, Action{T1, T2, T3, T4, T5}, Action)" />
        public static Command Create<T1, T2>(string name, string aliases, string helpText, Parameter p1, Parameter p2, Action<T1, T2> callback, Action defaultCallback = null)
        {
            return new Command()
            {
                Name = name,
                Aliases = aliases.Split(','),
                HelpText = helpText,
                Parameters = new Parameter[] { p1.SetType<T1>(), p2.SetType<T2>() },
                Callback = o => callback((T1)o[0], (T2)o[1]),
                DefaultCallback = defaultCallback
            };
        }

        /// <inheritdoc cref="Create{T1, T2, T3, T4, T5}(string, string, string, Parameter, Parameter, Parameter, Parameter, Parameter, Action{T1, T2, T3, T4, T5}, Action)" />
        public static Command Create<T1, T2, T3>(string name, string aliases, string helpText, Parameter p1, Parameter p2, Parameter p3, Action<T1, T2, T3> callback, Action defaultCallback = null)
        {
            return new Command()
            {
                Name = name,
                Aliases = aliases.Split(','),
                HelpText = helpText,
                Parameters = new Parameter[] { p1.SetType<T1>(), p2.SetType<T2>(), p3.SetType<T3>() },
                Callback = o => callback((T1)o[0], (T2)o[1], (T3)o[2]),
                DefaultCallback = defaultCallback
            };
        }

        /// <inheritdoc cref="Create{T1, T2, T3, T4, T5}(string, string, string, Parameter, Parameter, Parameter, Parameter, Parameter, Action{T1, T2, T3, T4, T5}, Action)" />
        public static Command Create<T1, T2, T3, T4>(string name, string aliases, string helpText, Parameter p1, Parameter p2, Parameter p3, Parameter p4, Action<T1, T2, T3, T4> callback, Action defaultCallback = null)
        {
            return new Command()
            {
                Name = name,
                Aliases = aliases.Split(','),
                HelpText = helpText,
                Parameters = new Parameter[] { p1.SetType<T1>(), p2.SetType<T2>(), p3.SetType<T3>(), p4.SetType<T4>() },
                Callback = o => callback((T1)o[0], (T2)o[1], (T3)o[2], (T4)o[3]),
                DefaultCallback = defaultCallback
            };
        }

        /// <summary>
        ///     Create a new command with parameters.
        /// </summary>
        /// <param name="name">Name used to call the command (e.g. "print").</param>
        /// <param name="aliases">Optional names that can be used to call the command, seperated by commas (e.g. "display,say").</param>
        /// <param name="helpText">Description of the command (e.g. "Display a message in the developer console").</param>
        /// <param name="p1">Parameter information (e.g. Parameter.Create("message", "Message to display in the developer console").</param>
        /// <param name="p2">Parameter information (e.g. Parameter.Create("message", "Message to display in the developer console").</param>
        /// <param name="p3">Parameter information (e.g. Parameter.Create("message", "Message to display in the developer console").</param>
        /// <param name="p4">Parameter information (e.g. Parameter.Create("message", "Message to display in the developer console").</param>
        /// <param name="p5">Parameter information (e.g. Parameter.Create("message", "Message to display in the developer console").</param>
        /// <param name="callback">Callback to invoke when the command is called with all parameters.</param>
        /// <param name="defaultCallback">Callback to invoke if the command is called with no parameters specified.</param>
        /// <returns></returns>
        public static Command Create<T1, T2, T3, T4, T5>(string name, string aliases, string helpText, Parameter p1, Parameter p2, Parameter p3, Parameter p4, Parameter p5, Action<T1, T2, T3, T4, T5> callback, Action defaultCallback = null)
        {
            return new Command()
            {
                Name = name,
                Aliases = aliases.Split(','),
                HelpText = helpText,
                Parameters = new Parameter[] { p1.SetType<T1>(), p2.SetType<T2>(), p3.SetType<T3>(), p4.SetType<T4>(), p5.SetType<T5>() },
                Callback = o => callback((T1)o[0], (T2)o[1], (T3)o[2], (T4)o[3], (T5)o[4]),
                DefaultCallback = defaultCallback
            };
        }

        /// <summary>
        ///     Create a new command instance using a command attribute.
        /// </summary>
        /// <param name="commandAttribute"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        internal static Command Create(DevConsoleCommandAttribute commandAttribute, MethodInfo method)
        {
            ParameterInfo[] parameterInfos = method.GetParameters();
            Parameter[] parameters = new Parameter[parameterInfos.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = Parameter.Create(parameterInfos[i].Name, i < commandAttribute.ParameterHelpText.Length ? commandAttribute.ParameterHelpText[i] : "");
                parameters[i].SetType(parameterInfos[i].ParameterType);
            }

            return new Command()
            {
                Name = commandAttribute.Name,
                Aliases = commandAttribute.Aliases,
                HelpText = commandAttribute.HelpText,
                Parameters = parameters,
                Callback = o => method.Invoke(null, o)
            };
        }

        #endregion

        #region Constructors

        private Command()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Name of the command.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Description of the command.
        /// </summary>
        public string HelpText { get; private set; }

        /// <summary>
        ///     Whether the command is a custom command (not a built-in command).
        /// </summary>
        public bool IsCustomCommand { get; private set; }

        /// <summary>
        ///     Optional command names.
        /// </summary>
        internal string[] Aliases { get; private set; }

        /// <summary>
        ///     Array of parameters required to execute the command.
        /// </summary>
        internal Parameter[] Parameters { get; private set; }

        /// <summary>
        ///     Callback to invoke when the command is executed.
        /// </summary>
        internal Action<object[]> Callback { get; private set; }

        /// <summary>
        ///     Default callback to invoke when the command is executed with no parameters.
        ///     This is only used for commands that require parameters.
        /// </summary>
        internal Action DefaultCallback { get; private set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///     Get the command syntax as a formatted string.
        /// </summary>
        /// <returns></returns>
        public string ToFormattedString()
        {
            string result = GetFormattedName();
            for (int i = 0; i < Parameters.Length; i++)
            {
                if (i < Parameters.Length)
                {
                    result += " ";
                }

                result += GetFormattedParameter(i);
            }
            return result;
        }

        /// <summary>
        ///     Get the command name as a formatted string.
        /// </summary>
        /// <returns></returns>
        public string GetFormattedName()
        {
            return $"<b>{Name}</b>";
        }

        /// <summary>
        ///     Get a parameter as a formatted string.
        /// </summary>
        /// <param name="parameterIndex"></param>
        /// <returns></returns>
        public string GetFormattedParameter(int parameterIndex)
        {
            return Parameters[parameterIndex].ToFormattedString();
        }

        /// <summary>
        ///     Get the command aliases.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<string> GetAliases()
        {
            return Aliases;
        }

        /// <summary>
        ///     Get the command parameters.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Parameter> GetParameters()
        {
            return Parameters;
        }

        /// <summary>
        ///     Ensure the command name is valid.
        /// </summary>
        internal void FixName()
        {
            Name = string.Concat(Name.Where(c => !char.IsWhiteSpace(c))).ToLower();
        }

        /// <summary>
        ///     Ensure the optional command names are valid.
        /// </summary>
        internal void FixAliases()
        {
            List<string> aliases = new List<string>();
            Array.ForEach(Aliases, a =>
            {
                string alias = string.Concat(a.Where(c => !char.IsWhiteSpace(c))).ToLower();
                if (!string.IsNullOrEmpty(alias))
                {
                    aliases.Add(alias);
                }
            });
            if (aliases.Count == 0)
            {
                aliases.Add("");
            }
            Aliases = aliases.ToArray();
        }

        /// <summary>
        ///     Check if the command conflicts any of the given aliases.
        /// </summary>
        /// <param name="aliases"></param>
        /// <returns></returns>
        internal bool HasAlias(params string[] aliases)
        {
            return aliases.Length > 0 && aliases.Any(a => !string.IsNullOrEmpty(a) && Aliases.Contains(a.ToLower()));
        }

        /// <summary>
        ///     Set whether the command is a custom command.
        /// </summary>
        internal void SetAsCustomCommand()
        {
            IsCustomCommand = true;
        }

        #endregion
    }
}
