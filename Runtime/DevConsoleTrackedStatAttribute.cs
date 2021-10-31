// File: DevConsoleTrackedStatAttribute.cs
// Purpose: Defines an attribute that is tracked by the developer console and displayed on-screen
// Created by: DavidFDev

using System;

namespace DavidFDev.DevConsole
{
    /// <summary>
    ///     Declare a static field or property to be tracked as a developer console stat.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DevConsoleTrackedStatAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        ///     Track a field or property and have it displayed on-screen.
        /// </summary>
        /// <param name="startEnabled">Whether to have the stat displayed by default.</param>
        public DevConsoleTrackedStatAttribute(bool startEnabled = true)
        {
            StartEnabled = startEnabled;
        }

        /// <summary>
        ///     Track a field or property and have it displayed on-screen.
        /// </summary>
        /// <param name="name">Custom name to use for the tracked stat.</param>
        /// <param name="startEnabled">Whether to have the stat displayed by default.</param>
        public DevConsoleTrackedStatAttribute(string name, bool startEnabled = true)
        {
            Name = name;
            StartEnabled = startEnabled;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Custom name to use for the tracked stat, otherwise use the target name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Whether the stat is displayed by default.
        /// </summary>
        public bool StartEnabled { get; }

        #endregion
    }
}
