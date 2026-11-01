using LocalImageGeneratorAI.Web.Services;

namespace LocalImageGeneratorAI.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();


            /*
            builder.Services.AddScoped<ComfyUiService>();// selbst eingefügt ;)
                      //Was bedeutet AddScoped? ASP.NET Core erstellt den Service bei
                      //Bedarf und verwendet dieselbe Instanz innerhalb eines HTTP-Requests.
                      //Bei einem neuen Request kann eine neue Instanz erstellt werden.*/


            builder.Services.AddHttpClient<ComfyUiService>(client =>
            {
                client.BaseAddress = new Uri("http://127.0.0.1:8188/");
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
