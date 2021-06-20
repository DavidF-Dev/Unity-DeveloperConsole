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

        internal string Name { get; }

        internal string[] Aliases { get; }

        internal string HelpText { get; }

        internal string[] ParameterHelpText { get; }

        #endregion
    }
}
