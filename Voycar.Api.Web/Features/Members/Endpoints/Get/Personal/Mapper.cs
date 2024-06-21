namespace Voycar.Api.Web.Features.Members.Endpoints.Get.Personal;

using Entities;
using Plans.Repository;


public class Mapper : ResponseMapper<Response, Member>
{
    private readonly IPlans _planRepository;


    public Mapper(IPlans planRepository)
    {
        this._planRepository = planRepository;
    }


    public Response FromEntities(Member member, User user)
    {
        return new Response
        {
            MemberId = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Street = member.Street,
            HouseNumber = member.HouseNumber,
            PostalCode = member.PostalCode,
            City = member.City,
            Country = member.Country,
            BirthDate = member.BirthDate,
            BirthPlace = member.BirthPlace,
            PhoneNumber = member.PhoneNumber,
            Email = user.Email,
            PlanName = this._planRepository.Retrieve(member.PlanId)!.Name
        };
    }
}