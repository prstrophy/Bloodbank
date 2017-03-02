using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BloodBank.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string MessageBody { get; set; }
        public DateTime CreatedDate { get; set; }

        
        public int MessageToId { get; set; }

        public virtual Users User { get; set; }
        public virtual Users MessageTo { get; set; }
    }
}