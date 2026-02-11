using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.AspNetCore.SignalR;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Extensions;
using Play.Catalog.Service.Repositories;

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
        private readonly IRepository<Item> itemsRepository ;

        public ItemsController(IRepository<Item>     itemsRepository)
        {
            this.itemsRepository = itemsRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> Get()
        {

            var items = (await itemsRepository.GetAllAsync()).Select(item => item.AsDto());
            if (items == null)
            {
                throw new Exception("Items not found");
            }

            return items;
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
            return NoContent();
        }

    }
}