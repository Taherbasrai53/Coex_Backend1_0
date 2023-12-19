using COeX_India1._0.Repositories;
using Quartz;

namespace COeX_India1._0.Models
{
    public class ActivityJob : IJob
    {
        private readonly IActivityRepo _activityRepo;

        public ActivityJob(IActivityRepo activityRepo)
        {
            _activityRepo = activityRepo;
            Console.WriteLine("Hello From The outside");
        }

        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Hello");
            Console.WriteLine(DateTime.UtcNow);
            Task response = _activityRepo.ExecuteCronJob();
            return response;
        }
    }
}
