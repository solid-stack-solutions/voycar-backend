namespace Voycar.Api.Web.Generic.Endpoint.Get.Single;

using Microsoft.AspNetCore.Http.HttpResults;

public abstract class Single<TEntity, TMapper>
    : Endpoint<Request, Results<Ok<Response<TEntity>>, NotFound>>, TMapper>
    where TMapper : ResponseMapper<Ok<>,>
{
}
