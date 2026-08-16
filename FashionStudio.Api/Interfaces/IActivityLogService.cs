using FashionStudio.Api.Models;

namespace FashionStudio.Api.Interfaces 
{
    public interface IActivityLogService 
    {
        public Task<ActivityLog> LogActivity(ActivityLog activity);
    }

}

