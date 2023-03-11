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
    public class TransferController : ControllerBase
    {
        private readonly IAccountDao accountDao;
        private readonly ITransferDao transferDao;

        public TransferController(IAccountDao _accountDao, ITransferDao _transferDao)
        {
            accountDao = _accountDao;
            transferDao = _transferDao;
        }

        [HttpGet("account/{userId}")]
        public IList<Transfer> GetAllTransfers(int userId)
        {
            Account account = accountDao.GetAccount(userId);
            return transferDao.GetAllTransfers(account.AccountId);
        }

        [HttpGet("{transferId}")]
        public ActionResult<Transfer> GetTransferDetailsById(int transferId)
        {
            Transfer transfer = transferDao.TransferDetails(transferId);
            if (transfer == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(transfer);
            }
        }

        [HttpPost]
        public ActionResult<Transfer> SendTransfer(Transfer transfer)
        {
            Transfer added = transferDao.SendTransfer(transfer.AccountFrom, transfer.AccountTo, transfer.Amount);
            return Created($"/transfer/{added.TransferId}", added);
        }

    }
}
