using Gerardr_Projet_NoSql.Models;

namespace Gerardr_Projet_NoSql.Interface
{
    public interface IProducts
    {
        public Task<List<Products>> GetAllOrders();
        public void AddOrder(Products prod);
        public void UpdateOrder(Products prod);
        public void DeleteOrder(string prodId);
    }
}
