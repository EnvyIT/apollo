using System.Runtime.CompilerServices;

namespace Apollo.Util.Logger
{
    public static class ApolloLoggerExtension
    {
        public static IApolloLogger<T> Here<T>(this IApolloLogger<T> logger, [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            logger
                .ForContext("MemberName", memberName)
                .ForContext("FilePath", sourceFilePath)
                .ForContext("LineNumber", sourceLineNumber);
            return logger;
        }
    }
}