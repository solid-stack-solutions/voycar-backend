namespace Voycar.Api.Web.Features.Members.Endpoints.Get.Personal;

using Entities;
using Plans.Repository;


public class Mapper : ResponseMapper<Response, Member>
{
    private readonly IPlans _repository;


    public Mapper(IPlans repository)
    {
        this._repository = repository;
    }


    public override Response FromEntity(Member e)
    {
        return new Response
        {
            FirstName = e.FirstName,
            LastName = e.LastName,
            Street = e.Street,
            HouseNumber = e.HouseNumber,
            PostalCode = e.PostalCode,
            City = e.City,
            Country = e.Country,
            BirthDate = e.BirthDate,
            BirthPlace = e.BirthPlace,
            PhoneNumber = e.PhoneNumber,
            PlanName = this._repository.Retrieve(e.PlanId)!.Name
        };
    }
}
