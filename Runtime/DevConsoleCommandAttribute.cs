// File: DevConsoleCommandAttribute.cs
// Purpose: Defines an attribute that can be used to add commands via method declarations
// Created by: DavidFDev

using System;

namespace DavidFDev.DevConsole
{
    /// <summary>
    ///     Declare a method as a command to be added to the dev console.
    ///     Method must be static, but can either be public or private.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class DevConsoleCommandAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        ///     Create a new command using the method arguments as parameters.
        /// </summary>
        /// <param name="name">Name used to call the command (e.g. "print").</param>
        /// <param name="aliases">Optional names that can be used to call the command, seperated by commas (e.g. "display,say").</param>
        /// <param name="helpText">Description of the command (e.g. "Display a message in the developer console").</param>
        /// <param name="onlyInDevBuild">Whether to only add the command if the project is a development build.</param>
        /// <param name="parameterHelpText">Descriptions of the parameters (e.g. "Message to display in the developer console").</param>
        public DevConsoleCommandAttribute(string name, string aliases, string helpText, bool onlyInDevBuild, params string[] parameterHelpText)
        {
            Name = name;
            Aliases = aliases.Split(',');
            HelpText = helpText;
            ParameterHelpText = parameterHelpText ?? new string[0];
            OnlyInDevBuild = onlyInDevBuild;
        }

        /// <inheritdoc cref="DevConsoleCommandAttribute(string, string, string, bool, string[])" />
        public DevConsoleCommandAttribute(string name, string aliases, string helpText, params string[] parameterHelpText)
            : this(name, aliases, helpText, false, parameterHelpText)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Name of the command.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        ///     Optional command names.
        /// </summary>
        internal string[] Aliases { get; }

        /// <summary>
        ///     Description of the command.
        /// </summary>
        internal string HelpText { get; }

        /// <summary>
        ///     Descriptions for the parameters.
        /// </summary>
        internal string[] ParameterHelpText { get; }

        internal bool OnlyInDevBuild { get; }

        #endregion
    }
}
