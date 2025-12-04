namespace VideoGameStore.Dtos
{
    public record class CustomerResponse(long Id, string Login, CartResponse CartResponse);
}
