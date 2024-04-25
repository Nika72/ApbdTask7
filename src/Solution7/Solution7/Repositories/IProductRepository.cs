namespace Solution7.Repositories
{
    public interface IProductRepository
    {
        Task<bool> Exists(int productId);
    }
}