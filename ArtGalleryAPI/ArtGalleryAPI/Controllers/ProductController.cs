﻿using ArtGalleryAPI.CustomExceptions;
using ArtGalleryAPI.Data;
using ArtGalleryAPI.Models.Domain;
using ArtGalleryAPI.Models.Dto;
using ArtGalleryAPI.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtGalleryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductInterface productService;
        public ProductController(IProductInterface productService)
        {
            this.productService = productService;
        }
        /// <summary>
        /// returns all the active products from the database
        /// </summary>
        /// <returns>list of all products</returns>

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// returns the filtered product record based on id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>filtered product</returns>
        [HttpGet]
        [Route("{productId:Guid}")]
        public async Task<IActionResult> GetProductById([FromRoute] Guid productId)
        {
            try
            {
                var product = await productService.GetProductByIdAsync(productId);
                if (product == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(product);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// add's a new product to db
        /// </summary>
        /// <param name="product"></param>
        /// <returns>new product</returns>
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] AddProductDto product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data provided!");
            }

            try
            {
                var newProduct = new Product
                {
                    Name = product.Name,
                    Description = product.Description,
                    ImageUrl = product.ImageUrl,
                    Price = product.Price,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                };
                await productService.CreateProductAsync(newProduct);
                var locationUri = Url.Action("GetProductById", new { productId = newProduct.ProductId });
                return Created(locationUri, newProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// updates the existing product in db
        /// </summary>
        /// <param name="updatedProduct"></param>
        /// <returns>updated product</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto updatedProduct)
        {
            try
            {
                var result = await productService.UpdateProductAsync(updatedProduct);
                if (result == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// delete a product in db based on id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>bool representing state of operation</returns>
        [HttpDelete]
        [Route("{productId:Guid}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid productId)
        {
            try
            {
                var deleteStatus = await productService.DeleteProductAsync(productId);
                return Ok(deleteStatus);
            }
            catch (InvalidDeletionException de)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
