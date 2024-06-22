namespace Voycar.Api.Web.Features.Users.Endpoints.Post.Register;

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
        this.RuleFor(request => request.Email)
            .NotEmpty().WithMessage("Your email is required!")
            .EmailAddress().WithMessage("Not a valid email address");

        this.RuleFor(request => request.Password)
            .NotEmpty().WithMessage("Your password is required!");

        this.RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("Your firstname is required!")
            .Length(2, 250).WithMessage("Your firstname is too short or too long!");

        this.RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("Your lastname is required!")
            .Length(2, 250).WithMessage("Your lastname is too short or too long!");

        this.RuleFor(request => request.Street)
            .NotEmpty().WithMessage("Your street is required!");

        this.RuleFor(request => request.HouseNumber)
            .NotEmpty().WithMessage("Your house number is required!");

        this.RuleFor(request => request.PostalCode)
            .NotEmpty().WithMessage("Your postal code is required!");

        this.RuleFor(request => request.City)
            .NotEmpty().WithMessage("Your place is required!");

        this.RuleFor(request => request.Country)
            .NotEmpty().WithMessage("Your country is required!");

        this.RuleFor(request
                => (DateOnly.FromDateTime(DateTime.UtcNow).DayNumber
                    - request.BirthDate.DayNumber) / 365)
            .GreaterThan(18).WithMessage("You are not legal yet!");

        this.RuleFor(request => request.BirthPlace)
            .NotEmpty().WithMessage("Your birth place is required!");

        this.RuleFor(request => request.PhoneNumber)
            .NotEmpty().WithMessage("Your phone number is required!");
    }
}
