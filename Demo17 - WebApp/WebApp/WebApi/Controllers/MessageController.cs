using MessageContracts;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class MessageController : ApiController
    {
        // GET api/message
        public async Task<IHttpActionResult> Get([FromUri] RequestMessageModel model)
        {
            
            await WebApiApplication.ServiceBus.Publish<RequestMessage>(model);
            return Ok();
        }

    }
}
