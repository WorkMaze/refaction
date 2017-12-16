using refactor_me.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace refactor_me.Interfaces
{
    /// <summary>
    /// The Product Factory interface which needs to be implmented by the individual data sources
    /// </summary>
    public interface IProductFactory
    {
        Task<Products> GetProducts();

        Task<Products> GetProducts(string name);

        Task<Product> GetProduct(Guid id);

        Task CreateProduct(Product product);

        Task UpdateProduct(Product product, Guid id);

        Task DeleteProduct(Guid id);

        Task<ProductOptions> GetProductOptions(Guid productId);

        Task<ProductOption> GetProductOption(Guid productId, Guid optionId);

        Task AddProductOption(Guid productId, ProductOption productOption);

        Task UpdateProductOption(Guid productId, Guid optionId,ProductOption productOption);

        Task DeleteProductOption(Guid productId,Guid optionId);
    }
}