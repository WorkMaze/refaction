using System;
using System.Net;
using System.Web.Http;
using refactor_me.Models;
using refactor_me.Factories;
using System.Configuration;
using System.Reflection;
using System.Net.Http;
using System.Threading.Tasks;
using refactor_me.Interfaces;

namespace refactor_me.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        private IProductFactory _productFactory;

        private IAuthorization _authorization;

        /// <summary>
        /// Initialize the Product Factory & Authorization class using reflection from Web.Config.
        /// </summary>
        public ProductsController()
        {
            // Product factory
            var productFactoryAppSetting = ConfigurationManager.AppSettings["IProductFactory"];
            if (!string.IsNullOrEmpty(productFactoryAppSetting))
            {                
                Type type = Type.GetType(productFactoryAppSetting);
                if (type == null)
                    throw new Exception(productFactoryAppSetting + " does not implement IProductFactory");
                else
                    _productFactory = (IProductFactory)Activator.CreateInstance(type);
            }
            else
                throw new Exception("AppSettings for IProductFactory not found in web.config");


            // Authorization class
            var authorizationAppSetting = ConfigurationManager.AppSettings["IAuthorization"];
            if (!string.IsNullOrEmpty(authorizationAppSetting))
            {
                Type type = Type.GetType(authorizationAppSetting);
                if (type == null)
                    throw new Exception(authorizationAppSetting + " does not implement IAuthorization");
                else
                    _authorization = (IAuthorization)Activator.CreateInstance(type);
            }
            else
                throw new Exception("AppSettings for IAuthorization not found in web.config");

        }

        /// <summary>
        /// GET /products
        /// </summary>
        /// <returns></returns>
        [Route]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            IHttpActionResult result = null;
            
            try
            {
                // Authorize
                var authorized = await _authorization.Authorize("Basic","Product","GET");
                if (!authorized)
                    throw new Exception("Unauthorized user access.");

                // Get Products
                var products = await _productFactory.GetProducts();
                if (products != null && products.Items != null
                    && products.Items.Count > 0)
                    result = Ok(products);

                else
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NoContent);
                    result = ResponseMessage(response);
                }
            }   
            catch(Exception ex)
            {
                result = InternalServerError(ex);
            }

            return result;        
        }

        /// <summary>
        /// GET /products?name={name}
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route]
        [HttpGet]
        public async Task<IHttpActionResult> SearchByName(string name)
        {
            IHttpActionResult result = null;

            try
            {
                // Authorize
                var authorized = await _authorization.Authorize("Basic", "Product", "GET");
                if (!authorized)
                    throw new Exception("Unauthorized user access.");


                // Get products by name
                var products = await _productFactory.GetProducts(name);
                if (products != null && products.Items != null
                    && products.Items.Count > 0)
                    result = Ok(products);

                else
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NoContent);
                    result = ResponseMessage(response);
                }
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }

            return result;
        }

        /// <summary>
        /// GET /products/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetProduct(Guid id)
        {
            IHttpActionResult result = null;

            try
            {
                // Authorize
                var authorized = await _authorization.Authorize("Basic", "Product", "GET");
                if (!authorized)
                    throw new Exception("Unauthorized user access.");

                // Get products by Id
                var product = await _productFactory.GetProduct(id);
                if (product != null)
                    result = Ok(product);
                else
                    result = NotFound();
                
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }

            return result;            

        }

        /// <summary>
        /// POST /products
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> Create(Product product)
        {
            IHttpActionResult result = null;

            try
            {
                // Authorize
                var authorized = await _authorization.Authorize("Basic", "Product", "POST");
                if (!authorized)
                    throw new Exception("Unauthorized user access.");

                // Create product
                await _productFactory.CreateProduct(product);
                var response = new HttpResponseMessage(HttpStatusCode.NoContent);
                result = ResponseMessage(response);
            }           
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }

            return result;
        }

        /// <summary>
        /// PUT /products/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> Update(Guid id, Product product)
        {
            IHttpActionResult result = null;

            try
            {
                // Authorize
                var authorized = await _authorization.Authorize("Basic", "Product", "PUT");
                if (!authorized)
                    throw new Exception("Unauthorized user access.");

                // Update product
                await _productFactory.UpdateProduct(product, id);
                var response = new HttpResponseMessage(HttpStatusCode.NoContent);
                result = ResponseMessage(response);
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }

            return result;
        }

        /// <summary>
        /// DELETE /products/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            IHttpActionResult result = null;

            try
            {
                // Authorize
                var authorized = await _authorization.Authorize("Basic", "Product", "DELETE");
                if (!authorized)
                    throw new Exception("Unauthorized user access.");

                // Delete product
                await _productFactory.DeleteProduct(id);
                var response = new HttpResponseMessage(HttpStatusCode.NoContent);
                result = ResponseMessage(response);
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }

            return result;
        }

        /// <summary>
        /// GET /products/{id}/options
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [Route("{productId}/options")]
        [HttpGet]
        public async Task<IHttpActionResult> GetOptions(Guid productId)
        {
            IHttpActionResult result = null;

            try
            {
                // Authorize
                var authorized = await _authorization.Authorize("Basic", "ProductOption", "GET");
                if (!authorized)
                    throw new Exception("Unauthorized user access.");

                // Get product options
                var productOptions = await _productFactory.GetProductOptions(productId);
                if (productOptions != null && productOptions.Items != null
                    && productOptions.Items.Count > 0)
                    result = Ok(productOptions);

                else
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NoContent);
                    result = ResponseMessage(response);
                }
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }

            return result;
        }

        /// <summary>
        /// GET /products/{id}/options/{optionId}
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{productId}/options/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetOption(Guid productId, Guid id)
        {
            IHttpActionResult result = null;

            try
            {
                // Authorize
                var authorized = await _authorization.Authorize("Basic", "ProductOption", "GET");
                if (!authorized)
                    throw new Exception("Unauthorized user access.");

                // Get product option
                var product = await _productFactory.GetProductOption(productId,id);
                if (product != null)
                    result = Ok(product);
                else
                    result = NotFound();

            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }

            return result;
        }

        /// <summary>
        /// POST /products/{id}/options
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        [Route("{productId}/options")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateOption(Guid productId, ProductOption option)
        {
            IHttpActionResult result = null;

            try
            {
                // Authorize
                var authorized = await _authorization.Authorize("Basic", "ProductOption", "POST");
                if (!authorized)
                    throw new Exception("Unauthorized user access.");

                // Create product option
                await _productFactory.AddProductOption(productId,option);
                var response = new HttpResponseMessage(HttpStatusCode.NoContent);
                result = ResponseMessage(response);
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }

            return result;
        }

        /// <summary>
        /// PUT /products/{id}/options/{optionId}
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="id"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        [Route("{productId}/options/{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateOption(Guid productId,Guid id, ProductOption option)
        {
            IHttpActionResult result = null;

            try
            {
                // Authorize
                var authorized = await _authorization.Authorize("Basic", "ProductOption", "PUT");
                if (!authorized)
                    throw new Exception("Unauthorized user access.");

                // Update product option
                await _productFactory.UpdateProductOption(productId,id, option);
                var response = new HttpResponseMessage(HttpStatusCode.NoContent);
                result = ResponseMessage(response);
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }

            return result;
        }

        /// <summary>
        /// DELETE /products/{id}/options/{optionId}
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{productId}/options/{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteOption(Guid productId,Guid id)
        {
            IHttpActionResult result = null;

            try
            {
                // Authorize
                var authorized = await _authorization.Authorize("Basic", "ProductOption", "DELETE");
                if (!authorized)
                    throw new Exception("Unauthorized user access.");


                //Delete product option
                await _productFactory.DeleteProductOption(productId,id);
                var response = new HttpResponseMessage(HttpStatusCode.NoContent);
                result = ResponseMessage(response);
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }

            return result;
        }
    }
}
