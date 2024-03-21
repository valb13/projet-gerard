using Gerardr_Projet_NoSql.DataAccessLayer;
using Gerardr_Projet_NoSql.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace BlazorMongoApp.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        ProductsDataAccessLayer objProducts = new ProductsDataAccessLayer(); // création de l'objet objProducts de la classe ProductsDataAccessLayer pour avoir accès aux fonctions de manipulation des bases de données 

        /// <summary>
        /// Récupération des produits depuis la bdd mongo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Get()
        {
            var data = objProducts.GetAllProducts().Result.ToList();
            var queryString = Request.Query;
            if (queryString.Keys.Contains("$inlinecount"))
            {
                StringValues Skip;
                StringValues Take;
                int skip = (queryString.TryGetValue("$skip", out Skip)) ? Convert.ToInt32(Skip[0]) : 0;
                int top = (queryString.TryGetValue("$top", out Take)) ? Convert.ToInt32(Take[0]) : data.Count();
                var count = data.Count();
                return new { Items = data.Skip(skip).Take(top), Count = count };
            }
            else
            {
                return data;
            }
        }

        /// <summary>
        /// Récupération d'un produit depuis la bdd mongo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<object> GetProd(string id)
        {
            try
            {
                var data = objProducts.GetProducts(id).Result;
                return data;
            }catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
 
        }

        /// <summary>
        /// Récupération des produits avec la commande SCAN depuis la bdd redis pour recherche dynamique 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("cache/scan/{key}")]
        public async Task<List<string>> GetScanCache(string key)
        {
            try
            {
                var scan = objProducts.GetSacnProductsRedis(key);
                return scan;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Récupération d'un produit depuis la bdd redis
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("cache/{id}")]
        public async Task<object> GetCache(string id)
        {
            try
            {
                var data = objProducts.GetProductsRedis(id);
                return data;

            }catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Ajout d'un produit dans la bdd mongo
        /// </summary>
        /// <param name="prod"></param>
        [HttpPost]
        public void Post([FromBody] Products prod)
        {
            objProducts.AddOrder(prod);
        }

        /// <summary>
        /// Ajout d'un produit dans la bdd redis
        /// </summary>
        /// <param name="prod"></param>
        [HttpPost("cache")]
        public void PostRedis([FromBody] Products prod)
        {
            objProducts.AddProductsRedis(prod);
        }

        /// <summary>
        /// Modification d'un produit dans la bdd mongo
        /// </summary>
        /// <param name="prod"></param>
        [HttpPut]
        public void Put([FromBody] Products prod)
        {
            objProducts.UpdateOrder(prod);
        }

        /// <summary>
        /// Suppression d'un produit dans la bdd mongo
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            objProducts.DeleteOrder(id);
        }
    }
}
