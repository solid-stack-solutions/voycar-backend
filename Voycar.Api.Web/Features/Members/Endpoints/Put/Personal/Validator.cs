namespace Voycar.Api.Web.Features.Members.Endpoints.Put.Personal;

public class Validator : Validator<Request>
{
    public Validator()
    {
        this.RuleFor(request => request.FirstName)
            .NotEmpty()
            .WithMessage("Firstname is required")
            .WithName("generalErrors")
            .Length(2, 250)
            .WithMessage("Firstname is too short or too long")
            .WithName("generalErrors");

        this.RuleFor(request => request.LastName)
            .NotEmpty()
            .WithMessage("Lastname is required")
            .WithName("generalErrors")
            .Length(2, 250)
            .WithMessage("Lastname is too short or too long")
            .WithName("generalErrors");

        this.RuleFor(request => request.Street)
            .NotEmpty()
            .WithMessage("Street is required")
            .WithName("generalErrors");

        this.RuleFor(request => request.HouseNumber)
            .NotEmpty()
            .WithMessage("House number is required")
            .WithName("generalErrors");

        this.RuleFor(request => request.PostalCode)
            .NotEmpty()
            .WithMessage("Postal code is required")
            .WithName("generalErrors");

        this.RuleFor(request => request.City)
            .NotEmpty()
            .WithMessage("City is required")
            .WithName("generalErrors");

        this.RuleFor(request => request.Country)
            .NotEmpty()
            .WithMessage("Country is required")
            .WithName("generalErrors");

        this.RuleFor(request =>
                (DateOnly.FromDateTime(DateTime.UtcNow).DayNumber - request.BirthDate.DayNumber) / 365)
            .GreaterThan(18)
            .WithMessage("You are not legal yet")
            .WithName("generalErrors");

        this.RuleFor(request => request.BirthPlace)
            .NotEmpty()
            .WithMessage("Birthplace is required")
            .WithName("generalErrors");

        this.RuleFor(request => request.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .WithName("generalErrors");
    }
}
