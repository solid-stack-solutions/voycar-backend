namespace Voycar.Api.Web.Features.Members.Endpoints.Get.Personal;

using Entities;
using Plans.Repository;


public class Mapper : ResponseMapper<Response, Member>
{
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
            PlanName = this.Resolve<IPlans>().Retrieve(member.PlanId)!.Name
        };
    }
}
