using System;
using System.Collections.Generic;

namespace Student.Models
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public Subgroup Subgroup { get; set; }

        public Subscription()
        {
            Id = Guid.NewGuid();
        }
    }
}