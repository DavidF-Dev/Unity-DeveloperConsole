using System;

namespace DavidFDev.DevConsole
{
    public sealed class Parameter
    {
        #region Static methods

        /// <summary>
        ///     Create a new parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="helpText"></param>
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

        internal Type Type { get; private set; }

        internal string Name { get; private set; }

        internal string HelpText { get; private set; }

        #endregion

        #region Methods

        internal Parameter SetType<T>()
        {
            Type = typeof(T);
            return this;
        }

        internal Parameter SetType(Type type)
        {
            Type = type;
            return this;
        }

        #endregion
    }
}
