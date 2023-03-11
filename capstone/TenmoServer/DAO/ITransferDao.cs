using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDao
    {
        Transfer TransferDetails(int transferId);
        IList<Transfer> GetAllTransfers(int accountId);
        Transfer SendTransfer(int fromUserId, int toUserId, decimal amount);
    }
}
