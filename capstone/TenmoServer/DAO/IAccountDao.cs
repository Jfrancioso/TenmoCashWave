using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDao
    {
        decimal GetBalance(int userId);
        Account GetAccount(int userId);
        decimal GetBalanceByAccount(int accountId);
    }
}
