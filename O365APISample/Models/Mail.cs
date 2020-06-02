using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace O365APISample.Models
{
    public class Inbox
    {
        public IEnumerable<MailItem> Mails { get; set; }
    }

    public class MailItem
    {
        public string id { get; set; }
        public string subject { get; set; }
        public DateTime receivedDateTime { get; set; }
        public MailFrom from { get; set; }
    }

    public class MailFrom
    {
        public MailEmailAddress emailAddress { get; set; }
    }

    public class MailEmailAddress
    {
        public string name { get; set; }
        public string address { get; set; }
    }
}