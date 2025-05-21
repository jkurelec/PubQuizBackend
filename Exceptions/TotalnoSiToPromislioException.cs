using System.Runtime.CompilerServices;

namespace PubQuizBackend.Exceptions
{
    public class TotalnoSiToPromislioException([CallerMemberName] string memberName = "",[CallerFilePath] string filePath = "",[CallerLineNumber] int lineNumber = 0)
        : Exception($"[{Path.GetFileNameWithoutExtension(filePath)}.{memberName}:{lineNumber}]")
    {
    }
}
