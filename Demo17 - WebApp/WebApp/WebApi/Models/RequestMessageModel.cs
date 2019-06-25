using MessageContracts;

namespace WebApi.Models
{
    public class RequestMessageModel : RequestMessage
    {
        public string Text { get; set; }
        public string To { get; set; }
    }
}