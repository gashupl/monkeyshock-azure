using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using MonkeyShock.Azure.WebApi.Model;

//Try tutorial: https://dev.to/berviantoleo/odata-with-net-6-5e1p

static IEdmModel GetEdmModel()
{
    ODataConventionModelBuilder builder = new();
    builder.EntitySet<Country>("Countries");
    builder.EntitySet<City>("Cities");
    return builder.GetEdmModel();
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MonkeyShock.Azure.WebApi.Model.AppContext>(
    (DbContextOptionsBuilder options) => options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);
builder.Services.AddControllers().AddOData(opt => opt.AddRouteComponents("v1", GetEdmModel())
    .Filter()
    .Select()
    .Expand()
    .Count()
    .SetMaxTop(250));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
