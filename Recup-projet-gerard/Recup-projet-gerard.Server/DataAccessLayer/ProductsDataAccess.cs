using Gerardr_Projet_NoSql.Interface;
using Gerardr_Projet_NoSql.Models;
using MongoDB.Driver;

namespace Gerardr_Projet_NoSql.DataAccessLayer
{
    public class ProductsDataAccessLayer : IProducts
    {
        private MongoClient mongoClient = null;
        private IMongoDatabase mongoDatabase = null;
        private IMongoCollection<Products> orderTable = null;

        public ProductsDataAccessLayer()
        {
            mongoClient = new MongoClient("mongodb://localhost:27017/");
            mongoDatabase = mongoClient.GetDatabase("Products");
            orderTable = mongoDatabase.GetCollection<Products>("products");
        }
        public async void AddOrder(Products prod)
        {
            try
            {
                await orderTable.InsertOneAsync(prod);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async void DeleteOrder(string prodId)
        {
            try
            {
                await orderTable.DeleteOneAsync(x => x.Id == prodId);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Products>> GetAllOrders()
        {
            try
            {
                var orders = orderTable.Find(FilterDefinition<Products>.Empty).ToListAsync();
                return await orders;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async void UpdateOrder(Products prod)
        {
            try
            {
                await orderTable.ReplaceOneAsync(x => x.Id == prod.Id, prod);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
