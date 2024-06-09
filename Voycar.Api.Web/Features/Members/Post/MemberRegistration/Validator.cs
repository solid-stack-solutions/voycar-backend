namespace Voycar.Api.Web.Features.Members.Post.MemberRegistration;

/// <summary>
/// Validator class for member registration requests.
///
/// Validates various properties of the registration request, such as personal information
/// and address details to ensure they meet the required criteria.
/// </summary>
public class Validator : Validator<Request>
{
    public Validator()
    {
        this.RuleFor(request => request.Email)
            .NotEmpty().WithMessage("your email is required!")
            .EmailAddress().WithMessage("not a valid email address");

        this.RuleFor(request => request.Password)
            .NotEmpty().WithMessage("your password is required!");

        this.RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("your firstname is required!")
            .Length(2, 250).WithMessage("your name is too short or too long!");

        this.RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("your firstname is required!")
            .Length(2, 250).WithMessage("your name is too short or too long!");

        this.RuleFor(request => request.Street)
            .NotEmpty().WithMessage("your street is required!");

        this.RuleFor(request => request.HouseNumber)
            .NotEmpty().WithMessage("your house number is required!");

        this.RuleFor(request => request.PostalCode)
            .NotEmpty().WithMessage("your postal code is required!");

        this.RuleFor(request => request.City)
            .NotEmpty().WithMessage("your place is required!");

        this.RuleFor(request => request.Country)
            .NotEmpty().WithMessage("your country is required!");

        this.RuleFor(request
                => (DateOnly.FromDateTime(DateTime.UtcNow).DayNumber
                    - request.BirthDate.DayNumber) / 365)
            .GreaterThan(18).WithMessage("you are not legal yet!");

        this.RuleFor(request => request.BirthPlace)
            .NotEmpty().WithMessage("your birth place is required!");

        this.RuleFor(request => request.PhoneNumber)
            .NotEmpty().WithMessage("your phone number is required!");
    }
}
