using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using MultiClientBlobStorage.Providers;
using MultiClientBlobStorage.Providers.GroupUserServices;
using MultiClientBlobStorage.Providers.Rbac;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

var services = builder.Services;
var configuration = builder.Configuration;

services.AddSingleton<ClientSecretCredentialProvider>();
services.AddScoped<ClientBlobContainerProvider>();
services.AddSingleton<AzureMgmtClientCredentialService>();
services.AddScoped<AzureMgmtClientService>();
services.AddSingleton<GraphApplicationClientService>();
services.AddScoped<ApplicationMsGraphService>();

services.AddHttpClient();
services.AddOptions();

builder.Services.AddDistributedMemoryCache();

services.AddMicrosoftIdentityWebAppAuthentication(configuration)
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

services.AddAuthorizationBuilder()
    .AddPolicy("blob-admin-policy", policyBlobOneRead =>
    {
        policyBlobOneRead.RequireClaim("roles", ["blobtwowriterole"]);
    });

services.AddRazorPages().AddMvcOptions(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

var app = builder.Build();

IdentityModelEventSource.ShowPII = true;
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
