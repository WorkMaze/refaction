using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using refactor_me.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using refactor_me.Interfaces;

namespace refactor_me.Factories
{
    /// <summary>
    /// SQL Server implementation for IProductFactory
    /// </summary>
    internal class SqlProductFactory : IProductFactory
    {
        protected string _connectionString;

        protected SqlConnection _connection;

        /// <summary>
        /// Reads the connection string from a JSON file
        /// </summary>
        public SqlProductFactory()
        {
            var sqlconfigText = File.ReadAllText(HttpContext.Current.Server.MapPath("~/") + "sqlconfig.json");

            if (string.IsNullOrEmpty(sqlconfigText))
                throw new Exception("Sql Server configuration not found.");

            var sqlConfiguration = JsonConvert.DeserializeObject<SqlConfiguration>(sqlconfigText);

            _connectionString = sqlConfiguration.ConnectionString;
        }

        /// <summary>
        /// Add Product Option
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="productOption"></param>
        /// <returns></returns>
        public async Task AddProductOption(Guid productId, ProductOption productOption)
        {
            try
            {
                var command = new SqlCommand("sp_ProductOption_Put", _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@Name", productOption.Name);
                command.Parameters.AddWithValue("@Description", productOption.Description);

                _connection.Open();
                await command.ExecuteNonQueryAsync();
            }           
            catch
            {
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    _connection.Close();
            }
        }

        /// <summary>
        /// Create Product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task CreateProduct(Product product)
        {
            try
            {
                var command = new SqlCommand("sp_Product_Put", _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@DeliveryPrice", product.DeliveryPrice);

                _connection.Open();
                await command.ExecuteNonQueryAsync();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    _connection.Close();
            }
        }

        /// <summary>
        /// Delete Product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteProduct(Guid id)
        {
            try
            {
                var command = new SqlCommand("sp_Product_Delete", _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id", id);

                _connection.Open();
                await command.ExecuteNonQueryAsync();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    _connection.Close();
            }
        }

        /// <summary>
        /// Delete Product Option
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="optionId"></param>
        /// <returns></returns>
        public async Task DeleteProductOption(Guid productId, Guid optionId)
        {
            try
            {
                SqlCommand command = new SqlCommand("sp_ProductOption_Delete", _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id", optionId);
                command.Parameters.AddWithValue("@ProductId", productId);

                _connection.Open();
                await command.ExecuteNonQueryAsync();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    _connection.Close();
            }
        }

        /// <summary>
        /// Get Product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Product> GetProduct(Guid id)
        {
            Product product = null;

            try
            {
                var command = new SqlCommand("sp_Product_GetById", _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id", id);

                _connection.Open();
                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    product = new Product()
                    {
                        Id = id,
                        Name = reader.GetString(0),
                        Description = reader.IsDBNull(1) ? null : reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        DeliveryPrice = reader.GetDecimal(3)
                    };

                    break;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    _connection.Close();
            }

            return product;
        }

        /// <summary>
        /// Get Product Option
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="optionId"></param>
        /// <returns></returns>
        public async Task<ProductOption> GetProductOption(Guid productId, Guid optionId)
        {
            ProductOption productOption = null;

            try
            {
                var command = new SqlCommand("sp_ProductOption_Get", _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@Id", optionId);

                _connection.Open();
                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    productOption = new ProductOption()
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ProductId = productId
                    };

                    break;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    _connection.Close();
            }

            return productOption;
        }

        /// <summary>
        /// Get Product Options
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<ProductOptions> GetProductOptions(Guid productId)
        {
            ProductOptions productOptions = null;

            try
            {
                var command = new SqlCommand("sp_ProductOption_Get", _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ProductId", productId);

                _connection.Open();
                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (productOptions == null)
                    {
                        productOptions = new ProductOptions();
                        productOptions.Items = new List<ProductOption>();
                    }

                    ProductOption product = new ProductOption()
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ProductId = productId
                    };

                    productOptions.Items.Add(product);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    _connection.Close();
            }

            return productOptions;
        }

        /// <summary>
        /// Get Products
        /// </summary>
        /// <returns></returns>
        public async Task<Products> GetProducts()
        {
            Products products = null;

            try
            {
                var command = new SqlCommand("sp_Product_GetByName", _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;


                _connection.Open();
                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (products == null)
                    {
                        products = new Products();
                        products.Items = new List<Product>();
                    }

                    Product product = new Product()
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Price = reader.GetDecimal(3),
                        DeliveryPrice = reader.GetDecimal(4)
                    };

                    products.Items.Add(product);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    _connection.Close();
            }

            return products;
        }

        /// <summary>
        /// Get Products by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Products> GetProducts(string name)
        {
            Products products = null;

            try
            {
                var command = new SqlCommand("sp_Product_GetByName", _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Name", name);

                _connection.Open();
                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    if (products == null)
                    {
                        products = new Products();
                        products.Items = new List<Product>();
                    }

                    Product product = new Product()
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Price = reader.GetDecimal(3),
                        DeliveryPrice = reader.GetDecimal(4)
                    };

                    products.Items.Add(product);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    _connection.Close();
            }

            return products;
        }

        /// <summary>
        /// Update Product
        /// </summary>
        /// <param name="product"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task UpdateProduct(Product product, Guid id)
        {
            try
            {
                var command = new SqlCommand("sp_Product_Put", _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@DeliveryPrice", product.DeliveryPrice);

                _connection.Open();
                await command.ExecuteNonQueryAsync();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    _connection.Close();
            }
        }

        /// <summary>
        /// Update Product Option
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="optionId"></param>
        /// <param name="productOption"></param>
        /// <returns></returns>
        public async Task UpdateProductOption(Guid productId, Guid optionId, ProductOption productOption)
        {
            try
            {
                var command = new SqlCommand("sp_ProductOption_Put", _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id", optionId);
                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@Name", productOption.Name);
                command.Parameters.AddWithValue("@Description", productOption.Description);

                _connection.Open();
                await command.ExecuteNonQueryAsync();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    _connection.Close();
            }
        }


        
    }
}