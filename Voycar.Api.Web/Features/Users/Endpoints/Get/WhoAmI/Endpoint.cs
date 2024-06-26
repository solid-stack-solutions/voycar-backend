namespace Voycar.Api.Web.Features.Users.Endpoints.Get.WhoAmI;

using Repository;

public class Endpoint : Endpoint<Request, Results<Ok<Response>, BadRequest<ErrorResponse>>, Mapper>
{
    private readonly IUsers _userRepository;

    public Endpoint(IUsers userRepository)
    {
        this._userRepository = userRepository;
    }


    public override void Configure()
    {
        this.Get("user/whoami");
        this.Roles("admin", "employee", "member");
        this.Summary(summary =>
        {
            summary.Summary = "Get Email of logged-in user";
            summary.Description = "Get the Email of the user who is logged in according to the request cookie";
            summary.Responses[200] = "If user is found";
            summary.Responses[400] = "If the user somehow can't be found";
        });
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = this._userRepository.Retrieve(req.UserId);
        if (user is null)
        {
            this.ThrowError("User does not exist");
        }

        await this.SendResultAsync(TypedResults.Ok(this.Map.FromEntity(user)));
    }
}
