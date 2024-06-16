namespace Voycar.Api.Web.Entities;

using Generic;

public class Plan : Entity
{
    public string Name { get; set; }
    public float MonthlyPrice { get; set; }
    public float HourlyPrice { get; set; }
}
