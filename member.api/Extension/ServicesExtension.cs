using member.api.Models;
using member.api.Shared.Facades;
using member.api.Shared.Repositories;
using member.api.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace member.api.Extension
{
    public static class ServicesExtension
    {
        public static void ConfigureScopeFacades(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddTransient<IAuthenFacades, AuthenFacades>();
            // services.AddTransient<IEmployeeFacades, EmployeeFacades>();
            // services.AddTransient<ITwoFactorFacades, TwoFactorFacades>();

        }
        public static void ConfigureScopeService(this IServiceCollection services, AppSettings appSettings)
        {
            var PublicKey = appSettings?.CredentialSetting?.PublicKey;
            var HashKey = appSettings.CredentialSetting.HashKey;
            var SecertKey = appSettings.CredentialSetting.SecertKey;
            var securityEncryption = new SecurityEncryption($"{PublicKey}");
            services.AddSingleton<ISecurityEncryption, SecurityEncryption>(x => securityEncryption);
            services.AddTransient<ISecurityService, SecurityService>(x => new SecurityService(securityEncryption, $"{HashKey}"));
            services.AddTransient<IJwtService, JwtService>(x => new JwtService($"{SecertKey}"));
            // services.AddScoped<IQrCodeGenerator, QrCodeGenerator>();
            // services.AddScoped<ICheckLoginFailFacade, CheckLoginFailFacade>();
            // services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        }
        public static void ConfigureScopeRepository(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddTransient<IMemberRepository, MemberRepository>(s => new MemberRepository(
                appSettings.CredentialSetting.MemberConnectionString,
                appSettings.CredentialSetting.SslMode,
                appSettings.CredentialSetting.Certificate
                ));
        }
    }
}