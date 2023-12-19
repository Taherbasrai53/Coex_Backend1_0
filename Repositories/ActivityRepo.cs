using COeX_India1._0.Data;
using COeX_India1._0.Helper;
using COeX_India1._0.Models;
using Microsoft.EntityFrameworkCore;

namespace COeX_India1._0.Repositories
{
    public interface IActivityRepo
    {
        public Task<Task> ExecuteCronJob();
    }
    public class ActivityRepo : IActivityRepo
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _dbContext;
        public ActivityRepo(IConfiguration config, ApplicationDbContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
        }
        public async Task<Task> ExecuteCronJob()
        {
            try
            {
                string logMsg = $"{Environment.NewLine}Activity Cron job started at : {DateTime.UtcNow}";
                Console.WriteLine(logMsg);

                var clusters = await _dbContext.Clusters.ToListAsync();
                foreach (var cluster in clusters)
                {
                    DataHelper dh = new DataHelper();
                    List<SqlPara> paras = new List<SqlPara>();
                    paras.Add(new SqlPara("@ClusterId", cluster.Id));
                    string sqlPara = @"
UPDATE Clusters
SET LiveRequests = (
    SELECT COUNT(*)
    FROM Mines
    WHERE ClusterId =@ClusterId  AND (TriggerYield - CurrYield) / YieldPerDay <= 2
), TentativelyAvailable= AvailableRakes 
WHERE Id = @ClusterId

Delete from requests where status=0
                ";
                    dh.ExecuteNonQuery(sqlPara, paras);
                }

                clusters= await _dbContext.Clusters.Where(c=> c.AvailableRakes<c.LiveRequests).ToListAsync();
                //List<Request> requestsList = new List<Request>();

                foreach (var cluster in clusters)
                {
                    DataHelper dh = new DataHelper();
                    List<SqlPara> paras = new List<SqlPara>();
                    paras.Add(new SqlPara("@ClusterId", cluster.Id));
                    string sqlExp = @"

--declare @ClusterId int=2
declare @CurrRakes int
declare @LiveReq int
declare @i int=0;

declare @RakeShortage int
select @RakeShortage= LiveRequests-AvailableRakes from Clusters where Id=@ClusterId

select M.Id as MineId, M.ClusterId, M.Lat, M.Long, (M.TriggerYield-M.CurrYield)/M.YieldPerDay as RemainingDays, 0 as Completed, Cluster.*
into #tmpCDAct
	from Mines M
	outer apply(
	Select Top(1) ((M.Lat- C.Lat)*(M.Lat- C.Lat) + (M.Long-C.Long)*(M.Long-C.Long)) as Dist,
	C.AvailableRakes/((C.LiveRequests+1)*((M.Lat- C.Lat)*(M.Lat- C.Lat) + (M.Long-C.Long)*(M.Long-C.Long))) as CostFunc , 
	C.Id as RequesteeClusterId
	from Clusters C where C.Id <>M.ClusterId and C.TentativelyAvailable>C.LiveRequests
	order by CostFunc desc
	) as Cluster
	where M.ClusterId=@ClusterId and M.AllocationStatus=0 and (M.TriggerYield-M.CurrYield)/M.YieldPerDay<= 2
	
	--select * from #tmpCDAct order by CostFunc desc
	  
	DECLARE @MineIdTmp INT, @ClusterIdTmp INT, @MineLat FLOAT, @MineLong FLOAT ,@RemainingDays FLOAT, @RequesteeClusterId INT, @Completed INT, @CostFunc FLOAT;

	DECLARE cursorName CURSOR FOR
    SELECT MineId, ClusterId, Lat, Long, RemainingDays, RequesteeClusterId, Completed, CostFunc
    FROM #tmpCDAct
    ORDER BY CostFunc DESC;

	OPEN cursorName;
	FETCH NEXT FROM cursorName INTO @MineIdTmp, @ClusterIdTmp, @MineLat, @MineLong, @RemainingDays, @RequesteeClusterId, @Completed, @CostFunc;
	
	while @@FETCH_STATUS =0
	Begin
	if @i<@RakeShortage
	begin
	select @CurrRakes=TentativelyAvailable, @LiveReq=LiveRequests from Clusters where Id=@RequesteeClusterId

	if @CurrRakes=@LiveReq
	begin 

	Select Top(1) (@MineLat- C.Lat)*(@MineLat- C.Lat) + (@MineLong-C.Long)*(@MineLong-C.Long) as Dist,
	C.AvailableRakes/((C.LiveRequests+1)*((@MineLat- C.Lat)*(@MineLat- C.Lat) + (@MineLong-C.Long)*(@MineLong-C.Long))) as CostFunc,
	C.Id as NewClusterId, C.TentativelyAvailable, C.liveRequests
	into #tmpCDAct2 
	from Clusters C where C.Id <> @ClusterIdTmp and C.Id <> @RequesteeClusterId and C.AvailableRakes> C.LiveRequests 
	order by CostFunc desc

	select @RequesteeClusterId=NewClusterId, @CurrRakes=TentativelyAvailable, @LiveReq=liveRequests from #tmpCDAct2

	end

	if @CurrRakes> @LiveReq
	begin
	Insert into Requests (SenderId, RecieverId, Priority, Status, InsertDate) values(@MineIdTmp, @RequesteeClusterId, 
	case when @RemainingDays <1 then 3 else 1 end, 0, GETDATE())	
	
	Update Clusters set TentativelyAvailable= @CurrRakes-1 where Id=@RequesteeClusterId
	end
	end
	else 
	begin
	Insert into Requests (SenderId, RecieverId, Priority, Status, InsertDate) values(@MineIdTmp, @ClusterIdTmp, 2, 0, GETDATE())
	end

	set @i=@i+1
	FETCH NEXT FROM cursorName INTO @MineIdTmp, @ClusterIdTmp, @MineLat, @MineLong, @RemainingDays, @RequesteeClusterId, @Completed, @CostFunc;

	End
	CLOSE cursorName;
    DEALLOCATE cursorName;
	--Select LiveRequests-AvailableRakes as OverflowVal from Clusters where Id=@ClusterId

	Drop table #tmpCDAct;
";
                    dh.Select(sqlExp, paras);
                    

                }

                return Task.CompletedTask;

            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }
    }
}
