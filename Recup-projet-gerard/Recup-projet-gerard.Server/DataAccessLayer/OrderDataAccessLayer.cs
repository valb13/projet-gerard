using Gerardr_Projet_NoSql.Interface;
using Gerardr_Projet_NoSql.Models;
using MongoDB.Driver;

namespace Gerardr_Projet_NoSql.DataAccessLayer
{
    public class OrderDataAccessLayer : IOrder
    {
        private MongoClient mongoClient = null;
        private IMongoDatabase mongoDatabase = null;
        private IMongoCollection<Order> orderTable = null;

        public OrderDataAccessLayer()
        {
            mongoClient = new MongoClient("mongodb://localhost:27017/");
            mongoDatabase = mongoClient.GetDatabase("Orders");
            orderTable = mongoDatabase.GetCollection<Order>("Orders");
        }
        public async void AddOrder(Order order)
        {
            try
            {
               await orderTable.InsertOneAsync(order);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async void DeleteOrder(string orderId)
        {
            try
            {
                await orderTable.DeleteOneAsync(x => x.Id == orderId);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Order>> GetAllOrders()
        {
            try
            {
                var orders = orderTable.Find(FilterDefinition<Order>.Empty).ToListAsync();
                return await orders;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async void UpdateOrder(Order order)
        {
            try
            {
               await orderTable.ReplaceOneAsync(x => x.Id == order.Id, order);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
