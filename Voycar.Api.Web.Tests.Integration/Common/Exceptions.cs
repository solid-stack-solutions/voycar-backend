namespace Voycar.Api.Web.Tests.Integration.Common;

/// <summary>
/// Serves as the base class for exceptions indicating missing items in the database, which are required
/// for program execution.
/// </summary>
public abstract class ElementNotInDbException : Exception
{
    protected ElementNotInDbException() : base() {}
    protected ElementNotInDbException(string message) : base(message) {}
    protected ElementNotInDbException(string message, Exception inner) : base(message, inner) {}
}

/// <summary>
/// The exception that is thrown when a specific role element is expected to be in the
/// database but is not actually there.
/// </summary>
public class RoleNotInDbException : ElementNotInDbException
{
    public RoleNotInDbException() {}
    public RoleNotInDbException(string message) : base(message) {}
    public RoleNotInDbException(string message, Exception inner) : base(message, inner) {}
}
