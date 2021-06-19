using System;
using System.Linq;
using System.Reflection;

namespace DavidFDev.DevConsole
{
    public sealed class Command
    {
        #region Static methods

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

        public static Command Create<T1>(string name, string aliases, string helpText, Parameter p1, Action<T1> callback, Action defaultCallback = null) where T1 : IConvertible
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

        public static Command Create<T1, T2>(string name, string aliases, string helpText, Parameter p1, Parameter p2, Action<T1, T2> callback, Action defaultCallback = null) where T1 : IConvertible where T2 : IConvertible
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

        public static Command Create<T1, T2, T3>(string name, string aliases, string helpText, Parameter p1, Parameter p2, Parameter p3, Action<T1, T2, T3> callback, Action defaultCallback = null) where T1 : IConvertible where T2 : IConvertible where T3 : IConvertible
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

        public static Command Create<T1, T2, T3, T4>(string name, string aliases, string helpText, Parameter p1, Parameter p2, Parameter p3, Parameter p4, Action<T1, T2, T3, T4> callback, Action defaultCallback = null) where T1 : IConvertible where T2 : IConvertible where T3 : IConvertible where T4 : IConvertible
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

        public static Command Create<T1, T2, T3, T4, T5>(string name, string aliases, string helpText, Parameter p1, Parameter p2, Parameter p3, Parameter p4, Parameter p5, Action<T1, T2, T3, T4, T5> callback, Action defaultCallback = null) where T1 : IConvertible where T2 : IConvertible where T3 : IConvertible where T4 : IConvertible where T5 : IConvertible
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

        internal static Command Create(DebugConsoleCommandAttribute commandAttribute, MethodInfo method)
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

        private Command() { }

        #endregion

        #region Properties

        internal string Name { get; private set; }

        internal string[] Aliases { get; private set; }

        internal string HelpText { get; private set; }

        internal Parameter[] Parameters { get; private set; }

        internal Action<object[]> Callback { get; private set; }

        internal Action DefaultCallback { get; private set; }

        #endregion

        #region Methods

        internal void FixName()
        {
            Name = string.Concat(Name.Where(c => !char.IsWhiteSpace(c))).ToLower();
        }

        internal bool HasAlias(params string[] aliases)
        {
            return aliases.Length > 0 && aliases.Any(a => !string.IsNullOrEmpty(a) && Aliases.Contains(a.ToLower()));
        }

        internal string GetFormattedName()
        {
            return "<b>" + Name + "</b>";
        }

        internal string GetFormattedParameter(Parameter parameter)
        {
            return "<i>(" + parameter.Type.Name + ")</i><b>" + parameter.Name + "</b>";
        }

        internal string ToFormattedString()
        {
            string result = GetFormattedName();
            for (int i = 0; i < Parameters.Length; i++)
            {
                if (i < Parameters.Length)
                {
                    result += " ";
                }

                result += GetFormattedParameter(Parameters[i]);
            }
            return result;
        }

        #endregion
    }
}
