using Azure;
using COeX_India1._0.Data;
using COeX_India1._0.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace COeX_India1._0.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class RequestsController:Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public RequestsController(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        [HttpGet("MyRequestsExternal")]
        [Authorize]

        public async Task<ActionResult> myRequests()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var claimUserId = claimsIdentity.FindFirst("userId")?.Value;
            var TokenUserId = 0;
            int.TryParse(claimUserId, out TokenUserId);
            var claimUserType = claimsIdentity.FindFirst("userType")?.Value;
            var TokenUserType = Models.User.EUserType.MineManager;
            
            Enum.TryParse(claimUserType, out TokenUserType);

            if(TokenUserType!=Models.User.EUserType.ClusterManager)
            {
                return BadRequest(new Models.Response(false, "Access Denied"));
            }
            var userCluster= await _dbContext.Users.Where(u=> u.UserId==TokenUserId).Select(u=> u.ClusterId).FirstOrDefaultAsync();
            
            //TimeSpan ts = TimeSpan.FromDays(2);

            var requestsQuery = from r in _dbContext.Requests
                           join m in _dbContext.Mines on r.SenderId equals m.Id into Alia1
                           from al1 in Alia1.DefaultIfEmpty()
                           join c in _dbContext.Clusters on al1.ClusterId equals c.Id into Alia2
                           from al2 in Alia2.DefaultIfEmpty()
                           where r.RecieverId== userCluster && r.Status==Models.Request.EStatus.Waiting && r.Priority!=Models.Request.EPriority.Mid
                           select new
                           {
                               r.Id,
                               MineSender = r.SenderId,
                               SenderName = al1.Name,
                               ClusterId = al1.ClusterId,
                               ClusterName = al2.Name,
                               r.Priority,
                               r.Status,
                               ExpectedIn = (al1.TriggerYield - al1.CurrYield) / al1.YieldPerDay
                           };

            var requestList = await requestsQuery.ToListAsync();

            return Ok(requestList);
        }

        [HttpGet("MyRequestsInternal")]
        [Authorize]

        public async Task<ActionResult> myRequestInternal()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var claimUserId = claimsIdentity.FindFirst("userId")?.Value;
            var TokenUserId = 0;
            int.TryParse(claimUserId, out TokenUserId);
            var claimUserType = claimsIdentity.FindFirst("userType")?.Value;
            var TokenUserType = Models.User.EUserType.MineManager;

            Enum.TryParse(claimUserType, out TokenUserType);

            if (TokenUserType != Models.User.EUserType.ClusterManager)
            {
                return BadRequest(new Models.Response(false, "Access Denied"));
            }
            var userCluster = await _dbContext.Users.Where(u => u.UserId == TokenUserId).Select(u => u.ClusterId).FirstOrDefaultAsync();
            
            //TimeSpan ts = TimeSpan.FromDays(2);

            var requestsQuery = from r in _dbContext.Requests
                                join m in _dbContext.Mines on r.SenderId equals m.Id into Alia1
                                from al1 in Alia1.DefaultIfEmpty()
                                join c in _dbContext.Clusters on al1.ClusterId equals c.Id into Alia2
                                from al2 in Alia2.DefaultIfEmpty()
                                where r.RecieverId == userCluster && r.Status == Models.Request.EStatus.Waiting && r.Priority == Models.Request.EPriority.Mid
                                select new
                                {
                                    r.Id,
                                    MineSender = r.SenderId,
                                    SenderName = al1.Name,
                                    ClusterId = al1.ClusterId,
                                    ClusterName = al2.Name,
                                    r.Priority,
                                    r.Status,
                                    ExpectedIn = (al1.TriggerYield - al1.CurrYield) / al1.YieldPerDay
                                };

            var requestList = await requestsQuery.ToListAsync();

            return Ok(requestList);
        }
    }
}
