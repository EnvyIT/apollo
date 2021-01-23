using System;

namespace Apollo.Util.Logger
{
    public interface IApolloLogger<T>
    {
        void Info(Exception exception, string messageTemplate, params object[] parameters);
        void Warning(Exception exception, string messageTemplate, params object[] parameters);
        void Error(Exception exception, string messageTemplate, params object[] parameters);
        void Verbose(Exception exception, string messageTemplate, params object[] parameters);
        void Fatal(Exception exception, string messageTemplate, params object[] parameters);
        void Debug(Exception exception, string messageTemplate, params object[] parameters);

        void Info(string messageTemplate, params object[] parameters);
        void Warning(string messageTemplate, params object[] parameters);
        void Error(string messageTemplate, params object[] parameters);
        void Verbose(string messageTemplate, params object[] parameters);
        void Fatal(string messageTemplate, params object[] parameters);
        void Debug(string messageTemplate, params object[] parameters);

        IApolloLogger<T> ForContext(string propertyName, object value);
    }
}
