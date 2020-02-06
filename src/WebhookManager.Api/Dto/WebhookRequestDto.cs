using WebhookManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookManager.Dto
{
  
    public class ApplicationCreateRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ApplicationId { get; set; }

        public IList<EventDto> Events { get; set; }


    }



    public class ApplicationCreateResponse
    {

        public string ApplicationId { get; set; }


    }


}
