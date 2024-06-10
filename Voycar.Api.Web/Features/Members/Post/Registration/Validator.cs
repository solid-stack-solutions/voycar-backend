namespace Voycar.Api.Web.Features.Members.Post.Registration;

/// <summary>
/// Validator class for user registration requests.
///
/// This class validates various properties of the registration request, such as email, password,
/// personal information, and address details to ensure they meet the required criteria.
/// </summary>
public class Validator : Validator<Request>
{
    public Validator()
    {
        this.RuleFor(user => user.Email)
            .NotEmpty().WithMessage("your email is required!")
            .EmailAddress().WithMessage("not a valid email address");

        this.RuleFor(user => user.FirstName)
            .NotEmpty().WithMessage("your firstname is required!")
            .Length(2, 250).WithMessage("your name is too short!");

        this.RuleFor(user => user.LastName)
            .NotEmpty().WithMessage("your firstname is required!")
            .Length(2, 250).WithMessage("your name is too short!");

        this.RuleFor(user => user.Street)
            .NotEmpty().WithMessage("your street is required!");

        this.RuleFor(user => user.HouseNumber)
            .NotEmpty().WithMessage("your house number is required!");

        this.RuleFor(user => user.PostalCode)
            .NotEmpty().WithMessage("your postal code is required!");

        this.RuleFor(user => user.City)
            .NotEmpty().WithMessage("your place is required!");

        this.RuleFor(user
                => (DateOnly.FromDateTime(DateTime.UtcNow).DayNumber
                    - user.BirthDate.DayNumber) / 365)
            .GreaterThan(18).WithMessage("you are not legal yet!");

        this.RuleFor(user => user.BirthPlace)
            .NotEmpty().WithMessage("your birth place is required!");

        this.RuleFor(user => user.PhoneNumber)
            .NotEmpty().WithMessage("your phone number is required!");

        this.RuleFor(user => user.DriversLicenseNumber)
            .NotEmpty().WithMessage("your drivers license number is required!");

        this.RuleFor(user => user.IdCardNumber)
            .NotEmpty().WithMessage("your id card number is required!");
    }
}
