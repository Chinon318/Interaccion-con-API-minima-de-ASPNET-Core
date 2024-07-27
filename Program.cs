using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MicrosoftAPI.Models.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();//Necesaria para APIS minimas
builder.Services.AddSwaggerGen();

//Agregar informacion para mostrarla en la documentacion de la API
builder.Services.AddSwaggerGen(option=>
{
    option.SwaggerDoc("v1",new  OpenApiInfo
    {
        Version = "v1", 
        Title = "Fruit API",
        Description = "API for managing a list of fruit their stock status.",
        TermsOfService = new Uri("https://example.com/terms")

    });
});

//Agregar base de datos
builder.Services.AddDbContext<FruitDb>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fruit API V1");
    options.RoutePrefix = string.Empty; 
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Agrega nueva fruta a la base de datos
app.MapPost("/fruitlist", async (Fruit fruit, FruitDb db) =>
{
    db.Fruits.Add(fruit);
    await db.SaveChangesAsync();

    return Results.Created($"/fruitlist/{fruit.Id}", fruit);
})
    .WithTags("Add fruit to list");

//Obtener todas las frutas de la lista
app.MapGet("fruitlist",async (FruitDb db)=>
{
    return await db.Fruits.ToListAsync();
})
    .WithTags("Get all fruits from list");

//Obtener frutas por ID
app.MapDelete("/fruitlist/{id}", async(int id, FruitDb db) =>
{
    var fruit = await db.Fruits.FindAsync(id);
    if (fruit == null)
    {
        return Results.NotFound();
    }
    db.Fruits.Remove(fruit);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
    .WithTags("Delete fruit by id");

app.Run();
