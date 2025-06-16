using Microsoft.Extensions.DependencyInjection;

namespace Gordon360.Services;

public static class ServicesExtensions
{
    /// <summary>
    /// Add our Gordon 360 Services as scoped services to the dependency injection container.
    /// </summary>
    /// <param name="services">Service container</param>
    /// <returns>The service container that has our services added.</returns>
    public static IServiceCollection Add360Services(this IServiceCollection services)
    {
        services.AddScoped<IAcademicCheckInService, AcademicCheckInService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IAddressesService, AddressesService>();
        services.AddScoped<IContentManagementService, ContentManagementService>();
        services.AddScoped<IDiningService, DiningService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IErrorLogService, ErrorLogService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IHousingService, HousingService>();
        services.AddScoped<IMembershipRequestService, MembershipRequestService>();
        services.AddScoped<IMembershipService, MembershipService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<ILostAndFoundService, LostAndFoundService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IStudentEmploymentService, StudentEmploymentService>();
        services.AddScoped<IVictoryPromiseService, VictoryPromiseService>();
        services.AddScoped<IPosterService, PosterService>();
        services.AddScoped<RecIM.IActivityService, RecIM.ActivityService>();
        services.AddScoped<RecIM.ISeriesService, RecIM.SeriesService>();
        services.AddScoped<RecIM.IMatchService, RecIM.MatchService>();
        services.AddScoped<RecIM.ITeamService, RecIM.TeamService>();
        services.AddScoped<RecIM.IParticipantService, RecIM.ParticipantService>();
        services.AddScoped<RecIM.ISportService, RecIM.SportService>();
        services.AddScoped<RecIM.IRecIMService, RecIM.RecIMService>();
        services.AddScoped<RecIM.IAffiliationService, RecIM.AffiliationService>();

        return services;
    }
}
