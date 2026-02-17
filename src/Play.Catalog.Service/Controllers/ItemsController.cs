using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Extensions;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{

    [ApiController, Route("items")]
    public class ItemsController : ControllerBase
    {
        // private static readonly List<ItemDto> items = new()
        //     {
        //     new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP",5, DateTimeOffset.UtcNow),
        //     new ItemDto(Guid.NewGuid(),"Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
        //     new ItemDto(Guid.NewGuid(),"Bronze sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)
        //     };
        private readonly IRepository<Item> itemsRepository;

        private readonly IPublishEndpoint publishEndpoint;

        // private static int requestCounter =0;

        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
        {
            this.itemsRepository = itemsRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            // requestCounter++;
            // Console.WriteLine($"Request{requestCounter}: Starting ....");

            // if(requestCounter <= 2)
            // {
            //     Console.WriteLine($"Request {requestCounter}: Delaying...");
            //     await Task.Delay(TimeSpan.FromSeconds(10));
            // }

            // if(requestCounter <= 4)
            // {
            //     Console.WriteLine($"Request {requestCounter}: 500(Internal Server Error)...");
            //     return StatusCode(500);
            // }

            var items = (await itemsRepository.GetAllAsync())
                                                .Select(item => item.AsDto());
            if (items == null)
            {
                throw new Exception("Items not found");
            }

           // Console.WriteLine($"Request {requestCounter} : 200(OK).");

            return Ok(items);
        }

        //   GET/Items/1
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {

            // var item = items.Where(item => item.Id == id).SingleOrDefault();
            // if (item == null)
            // {
            //     return NotFound();
            // }
            // return item;

            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }
            return item.AsDto();
        }

        // POST/items

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            // var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.UtcNow);
            // items.Add(item);
            // return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);

            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await itemsRepository.CreateAsync(item);

            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }


        // PUT/items/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            // var existingItem = items.Where(item => item.Id == id).SingleOrDefault();
            // if (existingItem == null)
            // {
            //     return NotFound();
            // }
            // var updatedItem = existingItem with
            // {
            //     Name = updateItemDto.Name,
            //     Description = updateItemDto.Description,
            //     Price = updateItemDto.Price
            // };

            // var index = items.FindIndex(existingItem => existingItem.Id == id);
            // items[index] = updatedItem;

            var existingItem = await itemsRepository.GetAsync(id);

            if (existingItem == null)
            {
                return NotFound();
            }
            else
            {

                var updated = existingItem with
                {
                    Name = updateItemDto.Name,
                    Description = updateItemDto.Description,
                    Price = updateItemDto.Price
                };

                await itemsRepository.UpdateAsync(updated);

                await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));
            }
            return NoContent();
        }


        // DELETE/items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            // 
            var item = await itemsRepository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            //await itemsRepository.DeleteAsync(item);
            await itemsRepository.RemoveAsync(item.Id);
            await publishEndpoint.Publish(new CatalogItemDeleted(id));
            return NoContent();
        }

    }
}