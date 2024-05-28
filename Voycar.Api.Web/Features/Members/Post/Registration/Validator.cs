namespace Voycar.Api.Web.Features.Members.Post.Registration;

using System.Text.RegularExpressions;

public class Validator : Validator<Request>
{
    public Validator()
    {
        this.RuleFor(member => member.Email)
            .NotEmpty().WithMessage("your email is required!")
            .EmailAddress().WithMessage("not a valid email address");


        this.RuleFor(member => member.Password)
            .Equal(member => member.ConfirmPassword)
            .WithMessage("passwords do not match")
            .Matches("[a-z]+").WithMessage("password must contain at least 1 lowercase letter")
            .Matches("[A-Z]+").WithMessage("password must contain at least 1 uppercase letter")
            .Matches("(\\d)+").WithMessage("password must contain at least 1 number")
            .Matches("(\\W)+").WithMessage("password must contain at least 1 symbol");


        this.RuleFor(member => member.FirstName)
            .NotEmpty().WithMessage("your firstname is required!")
            .Length(2, 250).WithMessage("your name is too short!");


        this.RuleFor(member => member.LastName)
            .NotEmpty().WithMessage("your firstname is required!")
            .Length(2, 250).WithMessage("your name is too short!");


        this.RuleFor(member
                => (DateOnly.FromDateTime(DateTime.UtcNow).DayNumber
                    - member.BirthDate.DayNumber) / 365)
            .GreaterThan(18).WithMessage("you are not legal yet!");

        // todo
        this.RuleFor(member => member.PhoneNumber)
            .NotEmpty().WithMessage("your phone number is required!");

        // todo
        this.RuleFor(member => member.Street)
            .NotEmpty().WithMessage("your street is required!");

        // todo
        this.RuleFor(member => member.HouseNumber)
            .NotEmpty().WithMessage("your house number is required!");


        // todo
        this.RuleFor(member => member.PostalCode)
            .NotEmpty().WithMessage("your postal code is required!");

        // todo
        this.RuleFor(member => member.Place)
            .NotEmpty().WithMessage("your place is required!");

        // todo
        this.RuleFor(member => member.BirthPlace)
            .NotEmpty().WithMessage("your birth place is required!");

        // todo
        this.RuleFor(member => member.DriversLicenseNumber)
            .NotEmpty().WithMessage("your DriversLicenseNumber is required!");

        // todo
        this.RuleFor(member => member.IdCardNumber)
            .NotEmpty().WithMessage("your IdCardNumber is required!");
    }
}
