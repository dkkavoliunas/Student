using System;
using System.Collections.Generic;

namespace Student.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Pasword { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
            Subscriptions = new List<Subscription>();
            Comments = new List<Comment>();
        }
    }
}
