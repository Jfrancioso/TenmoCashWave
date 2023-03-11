using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserDao userDao;
        private readonly IAccountDao accountDao;
        private readonly ITransferDao transferDao;

        public UserController(IUserDao _userDao, IAccountDao _accountDao, ITransferDao _transferDao) 
        {
            userDao = _userDao;
            accountDao = _accountDao;
            transferDao = _transferDao;
        }

        [HttpGet]
        public IList<User> GetUsers()
        {
            return userDao.GetUsers();
        }

        [HttpGet("{userId}")]
        public Account GetAccountByUserId(int userId)
        {
            return accountDao.GetAccount(userId);
        }

        [HttpGet("transfer/{transferId}/{accountId}")]
        public string GetUsernameByTransfer(int transferId, int accountId)
        {
            return userDao.GetUsernameByTransfer(transferDao.TransferDetails(transferId), accountId);
        }
    }
}
