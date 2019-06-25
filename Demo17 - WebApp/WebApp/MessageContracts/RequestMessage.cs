namespace MessageContracts
{
    public interface RequestMessage
    {
        string Text { get; set; }
        string To { get; set; }
    }
}
