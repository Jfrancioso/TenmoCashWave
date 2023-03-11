using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDao accountDao;

        public AccountController(IAccountDao _accountDao)
        {
            accountDao = _accountDao;
        }

        [HttpGet("{userid}")]
        public decimal GetBalance(int userId)
        {
            return accountDao.GetBalance(userId);
        }
        [HttpGet("/accountNum/{accountId}")]
        public decimal GetBalanceByAccount(int accountId)
        {
            return accountDao.GetBalanceByAccount(accountId);
        }
    }
}
