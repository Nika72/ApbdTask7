namespace Solution7.Repositories
{
    public interface IWarehouseRepository
    {
        Task<bool> Exists(int warehouseId);
    }
}