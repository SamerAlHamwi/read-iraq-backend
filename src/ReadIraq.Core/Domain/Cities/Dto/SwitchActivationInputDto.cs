namespace ReadIraq.Domain.Cities.Dto
{
    public class SwitchActivationInputDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
    public class ActivationStatuesDto
    {
        public bool IsActive { get; set; }
    }
    public class OutPutBooleanStatuesDto
    {
        public bool BooleanStatues { get; set; }
    }
    public class OutPutBooleanStatuesWithPriceDto : OutPutBooleanStatuesDto
    {
        public double FinalPrice { get; set; }
    }
}
