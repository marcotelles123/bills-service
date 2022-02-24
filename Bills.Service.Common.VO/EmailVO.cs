using System;
using System.Collections.Generic;
using System.Text;

namespace Bills.Service.Common.VO
{
    public class EmailVO
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public DateTime ReceivedDate { get; set; }
    }
}
