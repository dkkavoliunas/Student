using System.Collections.Generic;

namespace Student.Models
{
    public class User
    {
        public string Email { get; set; }
        public string Pasword { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }
}
