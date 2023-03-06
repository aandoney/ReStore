using API.Data;
using API.Dtos;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : BaseApiController
{
    private readonly StoreContext _context;

    private const string BuyerIdCookieName = "buyerId";

    public BasketController(StoreContext context)
    {
        _context = context;
    }

    [HttpGet(Name = "GetBasket")]
    public async Task<ActionResult<BasketDto>> GetBasket()
    {
        var basket = await RetrieveBasket();
        if (basket == null)
        {
            return NotFound();
        }

        return MapBasketToDto(basket);
    }

    [HttpPost]
    public async Task<ActionResult<BasketDto>> AddItemToBasket(int productId, int quantity)
    {
        var basket = await RetrieveBasket();
        if (basket == null)
        {
            basket = CreateBasket();
        }

        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            return NotFound();
        }

        basket.AddItem(product, quantity);

        var result = await _context.SaveChangesAsync() > 0;
        if (result == false)
        {
            return BadRequest(new ProblemDetails { Title = "Problem saving item to basket" });
        }

        return CreatedAtRoute("GetBasket", MapBasketToDto(basket));
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveBasketItem(int productId, int quantity)
    {
        var basket = await RetrieveBasket();
        if (basket == null)
        {
            return NotFound();
        }

        basket.RemoveItem(productId, quantity);

        var result = await _context.SaveChangesAsync() > 0;
        if (result == false)
        {
            return BadRequest(new ProblemDetails { Title = "Problem removing item from basket" });
        }

        return Ok();
    }

    private async Task<Basket> RetrieveBasket()
    {
        return await _context.Baskets
            .Include(i => i.Items)
            .ThenInclude(p => p.Product)
            .FirstOrDefaultAsync(x => x.BuyerId == Request.Cookies[BuyerIdCookieName]);
    }

    private Basket CreateBasket()
    {
        var buyerId = Guid.NewGuid().ToString();
        var cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTime.UtcNow.AddDays(30) };
        Response.Cookies.Append(BuyerIdCookieName, buyerId, cookieOptions);
        var basket = new Basket { BuyerId = buyerId };
        _context.Baskets.Add(basket);

        return basket;
    }

    private BasketDto MapBasketToDto(Basket basket)
    {
        return new BasketDto
        {
            Id = basket.Id,
            BuyerId = basket.BuyerId,
            Items = basket.Items.Select(item => new BasketItemDto
            {
                ProductId = item.ProductId,
                Name = item.Product.Name,
                Price = item.Product.Price,
                PictureUrl = item.Product.PictureUrl,
                Type = item.Product.Type,
                Brand = item.Product.Brand,
                Quantity = item.Quantity
            }).ToList()
        };
    }
}