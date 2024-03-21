using Gerardr_Projet_NoSql.Shared.Models;

namespace Gerardr_Projet_NoSql.Interface
{
    public interface IProducts
    {
        public Task<List<Products>> GetAllProducts();
        public Task<Products> GetProducts(string name);
        public void AddOrder(Products prod);
        public void UpdateOrder(Products prod);
        public void DeleteOrder(string prodId);
        public Products GetProductsRedis(string name);
        public void AddProductsRedis(Products prod);
        public List<string> GetSacnProductsRedis(string id);

    }
}
