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
        public string Id { get; set; }
        public string Subject { get; set; }
        public DateTime DateTimeReceived { get; set; }
        public MailFrom From { get; set; }
    }

    public class MailFrom
    {
        public MailEmailAddress EmailAddress { get; set; }
    }

    public class MailEmailAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}