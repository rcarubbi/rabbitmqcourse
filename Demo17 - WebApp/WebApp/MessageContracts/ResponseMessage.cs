namespace MessageContracts
{
    public interface ResponseMessage
    {
        string Answer { get; set; }

        string To { get; set; }
    }
}
