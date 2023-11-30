using Eventor.Core.Common.Seeds;
using System.Runtime.CompilerServices;

namespace Eventor.Core.Common.Validation;

public static class Check
{
    public static T ThrowIfNull<T>(T argument, [CallerArgumentExpression("argument")] string argumentName = "")

        => (argument is null)
                ? throw new ArgumentNullException(GlobalStrings.Argument_Null_Empty_Exception_Message, argumentName)
                    : argument;

}
