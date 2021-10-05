using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DevReviews.API.Entities;
using DevReviews.API.Models;
using DevReviews.API.Persistence;
using DevReviews.API.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DevReviews.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _repository.GetAllAsync();

          var productViewModel = _mapper.Map<List<ProductReviewViewModel>>(products);
            return Ok(productViewModel);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _repository.GetDetailsByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

          
            var productDetails = _mapper.Map<ProductDetailsViewModel>(product);
            return Ok(productDetails);
        }

        // POST para api/products
        /// <summary>Cadastro de Produtos</summary>
        /// <remarks>Requisição:
        /// {
        ///   "title": "Um tênis muito bom",
        ///   "description": "Um tênis de marca",
        ///   "price": 150
        /// }
        /// </remarks>
        /// <param name="model">Objeto com dados de cadastro do Produto</param>
        /// <returns>Objeto recém criado</returns>
        /// <response code= "201">Sucesso</response>
        /// <response code= "400">Dados Inválidos</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(AddProductInputModel model)
        {
            // Se tiver erros de validação, retornar BadRequest()
            var product = new Product(model.Title, model.Description, model.Price);

            Log.Information("Método POST chamado!");

            await _repository.AddAsync(product);

            return CreatedAtAction(nameof(GetById), new { id = 1 }, model);
        }

        // PUT para api/products/{id}
        [HttpPut("{id}")]

        public async Task<IActionResult> Put(int id, UpdateProductInputModel model)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            product.Update(model.Description, model.Price);

            await _repository.UpdateAsync(product);
            return NoContent();
        }
    }
}