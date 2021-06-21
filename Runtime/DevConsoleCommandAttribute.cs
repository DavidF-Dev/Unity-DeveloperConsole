﻿// File: DevConsoleCommandAttribute.cs
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

        public DevConsoleCommandAttribute(string name, string aliases, string helpText, params string[] parameterHelpText)
        {
            Name = name;
            Aliases = aliases.Split(',');
            HelpText = helpText;
            ParameterHelpText = parameterHelpText ?? new string[0];
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

        #endregion
    }
}