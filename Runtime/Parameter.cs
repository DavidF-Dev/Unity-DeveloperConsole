// File: Parameter.cs
// Purpose: Statically allows the creation of a parameter instance and internally holds the data
// Created by: DavidFDev

using System;

namespace DavidFDev.DevConsole
{
    /// <summary>
    ///     Parameter information used by a command.
    /// </summary>
    public sealed class Parameter
    {
        #region Static methods

        /// <summary>
        ///     Create a new parameter.
        /// </summary>
        /// <param name="name">Name of the parameter (e.g. "message").</param>
        /// <param name="helpText">Description of the parameter (e.g. "Message to display in the developer console").</param>
        /// <returns></returns>
        public static Parameter Create(string name, string helpText)
        {
            return new Parameter()
            {
                Name = name,
                HelpText = helpText
            };
        }

        #endregion

        #region Constructors

        private Parameter() { }

        #endregion

        #region Properties

        /// <summary>
        ///     IConvertible type of the parameter.
        /// </summary>
        internal Type Type { get; private set; }

        /// <summary>
        ///     Name of the parameter.
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        ///     Description of the parameter.
        /// </summary>
        internal string HelpText { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Set the internal type of the parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal Parameter SetType<T>()
        {
            Type = typeof(T);
            return this;
        }

        /// <summary>
        ///     Set the internal type of the parameter.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal Parameter SetType(Type type)
        {
            Type = type;
            return this;
        }

        #endregion
    }
}
