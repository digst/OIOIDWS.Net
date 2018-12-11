using System.Collections.Generic;

namespace Digst.OioIdws.TestDoubles
{
    public class RequestDescriptor
    {
        public string RequestMessageId { get; set; }

        public IList<string> RequestedAttributes { get; set; }

        public RequestorDescriptor Requestor { get; set; }
    }
}