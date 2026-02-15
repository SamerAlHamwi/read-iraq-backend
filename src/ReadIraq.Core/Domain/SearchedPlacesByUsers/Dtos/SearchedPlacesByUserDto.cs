namespace ReadIraq.Domain.SearchedPlacesByUsers.Dtos
{
    public class SearchedPlacesByUserDto
    {
        public string PlaceId { get; set; }
        public string AddressName { get; set; }
        public double lang { get; set; }
        public double lat { get; set; }
    }
}
