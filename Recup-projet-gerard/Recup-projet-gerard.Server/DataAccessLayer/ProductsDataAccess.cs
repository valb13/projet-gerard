using DevExpress.Blazor.Base;
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
            redisClient = ConnectionMultiplexer.Connect("localhost:10001,abortConnect=false");
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

        public Products GetProductsRedis(string name)
        {
            Products prod = new Products();
            var keys = new HashSet<string>();

            try
            {
                long nextCursor = 0;
                do
                {
                    var redisResult = redisDatabase.Execute("SCAN", nextCursor.ToString(), "MATCH", "*test*", "COUNT", "3");
                    var innerResult = (RedisResult[])redisResult;

                    nextCursor = long.Parse((string)innerResult[0]);

                    var resultLines = ((string[])innerResult[1]).ToArray();
                    keys.UnionWith(resultLines);
                }
                while (nextCursor != 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        

        var hashFields = redisDatabase.HashGetAll(name);
            prod.Name = hashFields[0].Value.ToString();
            prod.Description = hashFields[1].Value.ToString();
            prod.Price = hashFields[2].Value.ToString();
            prod.Stock = hashFields[3].Value.ToString();

            return prod;
        }

        public void AddProductsRedis(Products prod)
        {
            //ajouter un produit à la base redis 
            var hash = new HashEntry[] {
            new HashEntry("name", prod.Name),
            new HashEntry("description", prod.Description),
            new HashEntry("prix", prod.Price.ToString()),
            new HashEntry("stock", prod.Stock.ToString()),
            };

            redisDatabase.HashSet(prod.Name, hash);
            redisDatabase.KeyExpire(prod.Name, DateTime.Now.AddHours(1));
            

            var hashFields = redisDatabase.HashGetAll(prod.Name);
            Console.WriteLine("redis get");
            Console.WriteLine(String.Join("; ", hashFields));
        }
    }
}
