
using Microsoft.EntityFrameworkCore;
using VehicleMangement.EventStore;

namespace VehicleMangement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<Data.UserDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddMediatR(cfg =>cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            // EventRepository
            builder.Services.AddScoped<EventRepository>();
            builder.Services.AddAuthentication();
              //  JwtBearerDefaults.AuthenticationScheme)
              //.AddJwtBearer(options =>
              //{
              //    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
              //    {
              //        ValidateIssuer = true,
              //        ValidIssuer = builder.Configuration.GetValue<string>("JWT:Issuer"),
              //        ValidateAudience = true,
              //        ValidAudience = builder.Configuration.GetValue<string>("JWT:Audience"),
              //        ValidateLifetime = true,
              //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:SecretKey"))),
              //        ValidateIssuerSigningKey = true
              //    };
              //});


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.SetIsOriginAllowed(_ => true)

                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
