using JobIndexBuilder.Utilities;
using System;

namespace JobIndexBuilder.Services
{
    public class CommandLineOptionsService
    {
        public static CommandLineOptions GetCommandLineOptions(string[] args)
        {
            CommandLineOptions options;

            try
            {
                options = new CommandLineOptions(args, x =>
                {
                    x.AddOption("environment", "local");
                    x.AddOption("indexby", "alias");
                    x.AddOption("indexaliasvalues", "[jobs-de,jobs-en,jobs-es,jobs-fr,jobs-pt,jobs-zh]");
                    x.AddOption("indexnamevalues", "");
                });
            }
            catch (Exception ex)
            {
                ExceptionMessage(ex);
                return null;
            }
            return options;
        }

        /// <summary>
        /// Exception Message generator
        /// </summary>
        /// <param name="ex">Exception object</param>
        /// 
        private static void ExceptionMessage(Exception ex)
        {
        }

        /// <summary>
        /// Extracts all inner excetions and returns string representation.
        /// </summary>
        /// <param name="ex">Exception object</param>
        /// 
        private static string ExtractExceptionMessages(Exception ex)
        {
            var message = ex.Message;

            while (ex.InnerException != null)
            {
                var exception = ex.InnerException;
                message = message + " ---> Inner Exception: " + exception.Message;
                ex = exception;
            }
            return message;
        }
    }
}
