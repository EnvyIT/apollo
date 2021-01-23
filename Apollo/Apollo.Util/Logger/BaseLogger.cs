using System;
using Serilog;

namespace Apollo.Util.Logger
{
    public class ApolloLogger<T> : IApolloLogger<T>
    {
        private ILogger _logger;

        public ApolloLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Info(Exception exception, string messageTemplate, params object[] parameters)
        {
            _logger.Information(exception, messageTemplate, parameters);
        }

        public void Warning(Exception exception, string messageTemplate, params object[] parameters)
        {
            _logger.Warning(exception, messageTemplate, parameters);
        }

        public void Error(Exception exception, string messageTemplate, params object[] parameters)
        {
            _logger.Error(exception, messageTemplate, parameters);
        }

        public void Verbose(Exception exception, string messageTemplate, params object[] parameters)
        {
            _logger.Verbose(exception, messageTemplate, parameters);
        }

        public void Fatal(Exception exception, string messageTemplate, params object[] parameters)
        {
            _logger.Fatal(exception, messageTemplate, parameters);
        }

        public void Debug(Exception exception, string messageTemplate, params object[] parameters)
        {
            _logger.Debug(exception, messageTemplate, parameters);
        }

        public void Info(string messageTemplate, params object[] parameters)
        {
            _logger.Information(messageTemplate, parameters);
        }

        public void Warning(string messageTemplate, params object[] parameters)
        {
            _logger.Warning(messageTemplate, parameters);
        }

        public void Error(string messageTemplate, params object[] parameters)
        {
            _logger.Error(messageTemplate, parameters);
        }

        public void Verbose(string messageTemplate, params object[] parameters)
        {
            _logger.Verbose(messageTemplate, parameters);
        }

        public void Fatal(string messageTemplate, params object[] parameters)
        {
            _logger.Fatal(messageTemplate, parameters);
        }

        public void Debug(string messageTemplate, params object[] parameters)
        {
            _logger.Debug(messageTemplate, parameters);
        }

        public IApolloLogger<T> ForContext(string propertyName, object value)
        { 
            _logger = _logger.ForContext(propertyName, value);
            return this;
        }
    }
}