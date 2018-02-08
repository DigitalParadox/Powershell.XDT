using System;
using Microsoft.Web.XmlTransform;
using System.Management.Automation;

namespace Xdt.Powershell
{
   
    public class PowershellTransformationLogger : IXmlTransformationLogger
    {
        //HACK: hacky way to get logger to actually log to powershell properly 
        //
        private readonly Cmdlet _source;
        public PowershellTransformationLogger(Cmdlet sender)
        {
            _source = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public void LogMessage(string message, params object[] messageArgs)
        {
            _source.WriteObject(string.Format(message, messageArgs));
        }

        public void LogMessage(MessageType type, string message, params object[] messageArgs)
        {
            if (type == MessageType.Verbose)
            {
                _source.WriteVerbose(string.Format(message, messageArgs));
            }
            else
            {
                _source.WriteObject(string.Format(message, messageArgs));
            }
        }

        public void LogWarning(string message, params object[] messageArgs)
        {
            _source.WriteWarning(string.Format(message, messageArgs));
        }

        public void LogWarning(string file, string message, params object[] messageArgs)
        {
            _source.WriteWarning(string.Format($"{file} : {string.Format(message, messageArgs)}"));
        }

        public void LogWarning(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            _source.WriteWarning(string.Format($"{file} ({lineNumber},{linePosition}) : { string.Format(message, messageArgs) }"));
        }

        public void LogError(string message, params object[] messageArgs)
        {
            
            var exception = new ParseException(string.Format(message, messageArgs));
            _source.WriteError(new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.ReadError, exception.Message ));
        }

        public void LogError(string file, string message, params object[] messageArgs)
        {
            var exception = new ParseException($"{file} : { string.Format(message, messageArgs) }");

            _source.WriteError(new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.ReadError, exception.Message));
        }

        public void LogError(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            var exception = new ParseException($"{file}({lineNumber},{linePosition}) : { string.Format(message, messageArgs) }");

            _source.WriteError(new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.ReadError, exception.Message));
        }

        public void LogErrorFromException(Exception exception)
        {


            _source.WriteError(new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.ReadError, exception.Message));
        }

        public void LogErrorFromException(Exception ex, string file)
        {
            var exception = new ParseException($"{file} : {ex.Message}", ex);


            _source.WriteError(new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.ReadError, exception.Message));
        }

        public void LogErrorFromException(Exception ex, string file, int lineNumber, int linePosition)
        {
            var exception = new ParseException($"{file}({lineNumber},{linePosition}) : {ex.Message}", ex);


            _source.WriteError(new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.ReadError, exception.Message));

        }

        public void StartSection(string message, params object[] messageArgs)
        {
            _source.WriteObject(string.Format(message, messageArgs));
        }

        public void StartSection(MessageType type, string message, params object[] messageArgs)
        {
            if (type == MessageType.Verbose)
            {
                _source.WriteVerbose(string.Format(message, messageArgs));
            }
            else
            {
                _source.WriteObject(string.Format(message, messageArgs));
            }
        }

        public void EndSection(string message, params object[] messageArgs)
        {

            _source.WriteObject(string.Format(message, messageArgs));
        }

        public void EndSection(MessageType type, string message, params object[] messageArgs)
        {
            if (type == MessageType.Verbose)
            {
                _source.WriteVerbose(string.Format(message, messageArgs));
            }
            else
            {
                _source.WriteObject(string.Format(message, messageArgs));
            }
        }
    }
}
