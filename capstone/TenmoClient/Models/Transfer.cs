namespace TenmoServer.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public int TransferTypeId { get; set; }
        public int TransferStatusId { get; set; }
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public decimal Amount { get; set; }

        public Transfer() { }
        public Transfer(int transferId, int transferTypeId, int transferStatusId, int accountFrom, int accountTo, decimal amount) 
        {
            TransferId = transferId;
            TransferTypeId = transferTypeId;
            TransferStatusId = transferStatusId;
            AccountFrom = accountFrom;
            AccountTo = accountTo;
            Amount = amount;
        }
        public Transfer(int transferTypeId, int transferStatusId, int accountFrom, int accountTo, decimal ammount)
        {
            TransferTypeId = transferTypeId;
            TransferStatusId = transferStatusId;
            AccountFrom = accountFrom;
            AccountTo = accountTo;
            Amount = ammount;
        }
    }
}
