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

        this.RuleFor(request => request.BirthDate)
            .Must(BeAtLeast18YearsOld)
            .WithMessage("You must be at least 18 years old")
            .WithName("generalErrors");

        this.RuleFor(request => request.BirthPlace)
            .NotEmpty()
            .WithMessage("Birthplace is required")
            .WithName("generalErrors");

        this.RuleFor(request => request.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .WithName("generalErrors");

        this.RuleFor(request => request.PlanId)
            .NotEmpty()
            .WithMessage("Plan id is required")
            .WithName("generalErrors");
    }


    private static bool BeAtLeast18YearsOld(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - birthDate.Year;

        // Adjust age if the birthdate hasn't occurred yet this year
        if (today < birthDate.AddYears(age))
        {
            age--;
        }

        return age >= 18;
    }
}
