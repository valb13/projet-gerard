using Gerardr_Projet_NoSql.DataAccessLayer;
using Gerardr_Projet_NoSql.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace BlazorMongoApp.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        ProductsDataAccessLayer objProducts = new ProductsDataAccessLayer();

        [HttpGet]
        public async Task<object> Get()
        {
            var data = objProducts.GetAllOrders().Result.ToList();
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

        [HttpPost]
        public void Post([FromBody] Products prod)
        {
            objProducts.AddOrder(prod);
        }
        [HttpPut]
        public void Put([FromBody] Products prod)
        {
            objProducts.UpdateOrder(prod);
        }

        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            objProducts.DeleteOrder(id);
        }
    }
}
