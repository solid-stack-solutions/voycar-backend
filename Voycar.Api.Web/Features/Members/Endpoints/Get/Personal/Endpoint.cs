namespace Voycar.Api.Web.Features.Members.Endpoints.Get.Personal;

using Repository;
using Users.Repository;


public class Endpoint : Endpoint<Request, Results<Ok<Response>, BadRequest<NotFound>>, Mapper>
{
    private readonly IUsers _userRepository;
    private readonly IMembers _memberRepository;

    public Endpoint(IUsers userRepository, IMembers memberRepository)
    {
        this._userRepository = userRepository;
        this._memberRepository = memberRepository;
    }


    public override void Configure()
    {
        this.Get("member/personal");
        this.Roles("member");
        this.Summary(summary =>
        {
            summary.Summary = "Get personal details of logged in member";
            summary.Description = "Get the personal details of the member who is logged in according to the request cookie";
            summary.Responses[200] = "If member is found";
            summary.Responses[400] = "If the member somehow can't be found";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = this._userRepository.Retrieve(req.UserId);
        if (user is null)
        {
            await this.SendResultAsync(TypedResults.BadRequest("User does not exist"));
            return;
        }

        var member = this._memberRepository.Retrieve(user.MemberId.Value);
        await this.SendResultAsync(TypedResults.Ok(this.Map.FromEntity(member!)));
    }
}
