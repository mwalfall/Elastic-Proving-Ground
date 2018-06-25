using System;
using System.Collections.Generic;

namespace JobIndexBuilder.Utilities
{
    public class CommandLineOptions
    {
        private HashSet<string> optionSet = new HashSet<string>();
        private HashSet<string> options = new HashSet<string>();
        private Dictionary<string, Type> optionTypes = new Dictionary<string, Type>();
        private Dictionary<string, object> optionValues = new Dictionary<string, object>();
        private Dictionary<string, object> defaultValues = new Dictionary<string, object>();

        public CommandLineOptions(Action<CommandLineOptions> configure)
        {
            configure(this);
        }

        public CommandLineOptions(string[] args, Action<CommandLineOptions> configure)
        {
            configure(this);
            LoadArguments(args);
        }

        public void AddOption(string name)
        {
            if (!optionSet.Contains(name))
            {
                optionSet.Add(name);
            }
        }

        public void AddOption<T>(string name)
        {
            if (!optionTypes.ContainsKey(name))
            {
                optionSet.Add(name);
                optionTypes.Add(name, typeof(T));
            }
        }

        public void AddOption<T>(string name, T defaultValue)
        {
            AddOption<T>(name);

            if (!defaultValues.ContainsKey(name))
            {
                defaultValues.Add(name, defaultValue);
            }
        }

        public bool HasOption(string name)
        {
            return options.Contains(name);
        }

        public T GetOptionValue<T>(string name)
        {
            object value = null;

            if (optionValues.ContainsKey(name))
            {
                value = optionValues[name];
            }

            if (value == null)
            {
                if (defaultValues.ContainsKey(name))
                {
                    value = defaultValues[name];
                }
                else
                {
                    throw new ArgumentException(string.Format("Value not found for option: {0}", name));
                }
            }

            var optionType = optionTypes[name];

            if (optionType == typeof(T))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            else
            {
                throw new ArgumentException(string.Format("Type mismatch. Specified: {0}, Expected: {1}", typeof(T).ToString(), optionType.ToString()));
            }
        }

        public void LoadArguments(string[] args)
        {
            int index = 0;

            while (index < args.Length)
            {
                if (args[index].StartsWith("-"))
                {
                    var argName = args[index].TrimStart('-');

                    if (optionSet.Contains(argName))
                    {
                        if (optionTypes.ContainsKey(argName))
                        {
                            optionValues.Add(argName, args[index + 1]);
                            index += 2;
                        }
                        else
                        {
                            index++;
                        }

                        options.Add(argName);
                    }
                    else
                    {
                        throw new ArgumentException(string.Format("Unknown argument: {0}", argName));
                    }
                }
                else
                {
                    index++;
                }
            }
        }
    }
}
