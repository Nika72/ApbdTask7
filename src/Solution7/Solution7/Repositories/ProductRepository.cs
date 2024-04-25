namespace Solution7.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public async Task<bool> Exists(int productId)
        {
            // Database interaction logic
            return true;
        }
    }
}