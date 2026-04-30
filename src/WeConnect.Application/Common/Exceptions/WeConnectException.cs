namespace WeConnect.Application.Common.Exceptions;

public class WeConnectException      : Exception { public WeConnectException(string m) : base(m) { } }
public class NotFoundException       : WeConnectException { public NotFoundException(string m) : base(m) { } }
public class ValidationException     : WeConnectException
{
    public IEnumerable<(string Field, string Error)> Errors { get; }
    public ValidationException(IEnumerable<(string, string)> errors)
        : base("One or more validation errors occurred.")
        => Errors = errors;
}
public class UnauthorizedException   : WeConnectException { public UnauthorizedException(string m = "Unauthorized") : base(m) { } }
public class ForbiddenException      : WeConnectException { public ForbiddenException(string m = "Forbidden") : base(m) { } }
public class ConflictException       : WeConnectException { public ConflictException(string m) : base(m) { } }