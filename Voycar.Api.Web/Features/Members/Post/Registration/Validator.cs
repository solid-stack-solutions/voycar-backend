namespace Voycar.Api.Web.Features.Members.Post.Registration;

/// <summary>
/// Validator class for member registration requests.
///
/// This class validates various properties of the registration request, such as email, password,
/// personal information, and address details to ensure they meet the required criteria.
/// </summary>
public class Validator : Validator<Request>
{
    public Validator()
    {
        this.RuleFor(member => member.Email)
            .NotEmpty().WithMessage("your email is required!")
            .EmailAddress().WithMessage("not a valid email address");

        this.RuleFor(member => member.FirstName)
            .NotEmpty().WithMessage("your firstname is required!")
            .Length(2, 250).WithMessage("your name is too short!");

        this.RuleFor(member => member.LastName)
            .NotEmpty().WithMessage("your firstname is required!")
            .Length(2, 250).WithMessage("your name is too short!");

        this.RuleFor(member => member.Street)
            .NotEmpty().WithMessage("your street is required!");

        this.RuleFor(member => member.HouseNumber)
            .NotEmpty().WithMessage("your house number is required!");

        this.RuleFor(member => member.PostalCode)
            .NotEmpty().WithMessage("your postal code is required!");

        this.RuleFor(member => member.City)
            .NotEmpty().WithMessage("your place is required!");

        this.RuleFor(member
                => (DateOnly.FromDateTime(DateTime.UtcNow).DayNumber
                    - member.BirthDate.DayNumber) / 365)
            .GreaterThan(18).WithMessage("you are not legal yet!");

        this.RuleFor(member => member.BirthPlace)
            .NotEmpty().WithMessage("your birth place is required!");

        this.RuleFor(member => member.PhoneNumber)
            .NotEmpty().WithMessage("your phone number is required!");

        this.RuleFor(member => member.DriversLicenseNumber)
            .NotEmpty().WithMessage("your drivers license number is required!");

        this.RuleFor(member => member.IdCardNumber)
            .NotEmpty().WithMessage("your id card number is required!");
    }
}
