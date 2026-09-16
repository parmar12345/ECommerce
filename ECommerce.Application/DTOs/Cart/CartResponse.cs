namespace ECommerce.Application.DTOs.Cart;

public class CartResponse
{
    public Guid Id { get; set; }

    public List<CartItemResponse> Items { get; set; }
        = new();

    public int TotalItems { get; set; }

    public decimal Subtotal { get; set; }
}