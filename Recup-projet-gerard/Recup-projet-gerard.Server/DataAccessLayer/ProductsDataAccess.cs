using Gerardr_Projet_NoSql.Interface;
using Gerardr_Projet_NoSql.Models;
using MongoDB.Driver;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;

namespace Gerardr_Projet_NoSql.DataAccessLayer
{
    public class ProductsDataAccessLayer : IProducts
    {
        private MongoClient mongoClient = null;
        private ConnectionMultiplexer redisClient = null;
        private IMongoDatabase mongoDatabase = null;
        private IDatabase redisDatabase = null;
        private IMongoCollection<Products> prodTable = null;

        public ProductsDataAccessLayer()
        {
            mongoClient = new MongoClient("mongodb://localhost:27017/");
            mongoDatabase = mongoClient.GetDatabase("Products");
            prodTable = mongoDatabase.GetCollection<Products>("products");
            redisClient = ConnectionMultiplexer.Connect("localhost");
            redisDatabase = redisClient.GetDatabase();

        }
        public async void AddOrder(Products prod)
        {
            try
            {
                await prodTable.InsertOneAsync(prod);
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
                await prodTable.DeleteOneAsync(x => x.Id == prodId);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Products>> GetAllProducts()
        {
            try
            {
                var orders = prodTable.Find(FilterDefinition<Products>.Empty).ToListAsync();
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
                await prodTable.ReplaceOneAsync(x => x.Id == prod.Id, prod);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Products GetProductsRedis(string id)
        {
            var ft = redisDatabase.FT();  

            try
            {
                var res = ft.Search("idx:produits", new Query("id")).Documents.Select(x => x["json"]);
                Console.WriteLine(string.Join("\n", res));
                return (Products)res;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddProductsRedis(Products prod)
        {
            var schema = new Schema()
                .AddTextField(new FieldName("$.name", prod.Name))
                .AddTextField(new FieldName("$.price", prod.Price))
                .AddTextField(new FieldName("$.description", prod.Description))
                .AddTextField(new FieldName("$.stock", prod.Stock));

            var ft = redisDatabase.FT();

            ft.Create(
                "idx:produits",
                new FTCreateParams().On(IndexDataType.JSON).Prefix("produit:"), schema);
            
            var json = redisDatabase.JSON();

            json.Set("produit:" + prod.Id, "$", prod);
        }
    }
}
