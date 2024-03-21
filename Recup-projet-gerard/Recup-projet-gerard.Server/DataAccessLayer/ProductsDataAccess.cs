using DevExpress.Blazor.Base;
using Gerardr_Projet_NoSql.Interface;
using Gerardr_Projet_NoSql.Shared.Models;
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
            mongoClient = new MongoClient("mongodb://localhost:27017/"); //création de la connexion à la base de données mongo
            mongoDatabase = mongoClient.GetDatabase("Products"); //utilisation de la base de données Products
            prodTable = mongoDatabase.GetCollection<Products>("products"); //utilisation de la collection products
            redisClient = ConnectionMultiplexer.Connect("localhost:10001,abortConnect=false"); //création de la connexion à la base de données redis
            redisDatabase = redisClient.GetDatabase(); //utilisation de la base de données redis

        }

        /// <summary>
        /// Fonction d'ajout de produit dans la base mongo  
        /// </summary>
        /// <param name="prod"></param>
        public async void AddOrder(Products prod)
        {
            try
            {
                await prodTable.InsertOneAsync(prod); //ajout du produit dans la base mongo
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Fonction de suppression de produit dans la base mongo
        /// </summary>
        /// <param name="prodId"></param>
        public async void DeleteOrder(string prodId)
        {
            try
            {
                await prodTable.DeleteOneAsync(x => x.Id == prodId); //suppression du produit dans la base mongo

            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Fonction pour récupérer tous les produits de la base mongo
        /// </summary>
        /// <returns></returns>
        public async Task<List<Products>> GetAllProducts()
        {
            try
            {
                var orders = prodTable.Find(FilterDefinition<Products>.Empty).ToListAsync(); //récupération de tous les produits dans la base mongo
                return await orders;
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Fonction pour récupérer un produit de la base mongo
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Products> GetProducts(string name)
        {
            try
            {
                var res = await prodTable.Find(x => x.Name == name).FirstOrDefaultAsync(); //récupération du produit dans la base mongo
                if(res == null) { return new Products(); } //si le produit n'existe pas dans la base mongo, on retourne un produit vide
                else 
                    return res;
            
            }catch(Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        /// <summary>
        /// Fonction pour modifier un produit de la base mongo
        /// </summary>
        /// <param name="prod"></param>
        public async void UpdateOrder(Products prod)
        {
            try
            {
                await prodTable.ReplaceOneAsync(x => x.Id == prod.Id, prod); //remplacement du produit dans la base mongo
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Fonction pour récupérer un produit de la base redis
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Products GetProductsRedis(string name)
        {
            Products prod = new Products();

            try
            {
                var hashFields = redisDatabase.HashGetAll(name); //récupération des champs du produit dans la base redis avec la commande HGETALL

                //création d'un objet produit avec les champs récupérés dans la bdd redis
                prod.Name = hashFields[0].Value.ToString();
                prod.Description = hashFields[1].Value.ToString();
                prod.Price = hashFields[2].Value.ToString();
                prod.Stock = hashFields[3].Value.ToString();

                return prod;
            
            }catch(Exception e)
            {
                return prod;
            }
            
        }


        /// <summary>
        /// Fonction pour ajouter un produit dans la base redis
        /// </summary>
        /// <param name="prod"></param>
        public void AddProductsRedis(Products prod)
        {
            //ajouter un produit à la base redis 
            try
            {
                //création d'un tableau de champs pour le produit
                var hash = new HashEntry[] {

                    //ajout des champs du produit
                    new HashEntry("name", prod.Name),
                    new HashEntry("description", prod.Description),
                    new HashEntry("prix", prod.Price.ToString()),
                    new HashEntry("stock", prod.Stock.ToString()),
                };

                redisDatabase.HashSet(prod.Name, hash); //ajout du produit dans la base redis avec la commande HSET
                redisDatabase.KeyExpire(prod.Name, DateTime.Now.AddHours(1)); //expiration de la clé du produit après 1 heure


                var hashFields = redisDatabase.HashGetAll(prod.Name); //récupération des champs du produit ajouté dans la base redis
                Console.WriteLine("redis get"); 
                Console.WriteLine(String.Join("; ", hashFields)); //affichage des champs du produit ajouté dans la base redis
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        
        /// <summary>
        /// Fonction pour récupérer un produit de la base redis grâce à la commande SCAN
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<string> GetSacnProductsRedis(string id)
        {
            var keys = new HashSet<string>();
            List<string> keysScan = new List<string>();

            try
            {
                long nextCursor = 0;
                do
                {
                    var redisResult = redisDatabase.Execute("SCAN", nextCursor.ToString(), "MATCH", $"*{id}*", "COUNT", "5"); //récupération des clés des produits avec la commande SCAN
                    var innerResult = (RedisResult[])redisResult; //récupération des résultats de la commande SCAN

                    nextCursor = long.Parse((string)innerResult[0]); //récupération du prochain curseur

                    var resultLines = ((string[])innerResult[1]).ToArray(); //récupération des clés des produits
                    foreach(var line in resultLines) //ajout des clés des produits dans la liste keysScan
                    {
                        keysScan.Add(line);
                    }
                    keys.UnionWith(resultLines); //ajout des clés des produits dans le HashSet keys
                }
                while (nextCursor != 0);

                return keysScan; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<string>();
            }
        }

     
    }
}
