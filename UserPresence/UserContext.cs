using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace UserPresence
{
    public class UserContext : DbContext
    {
        public UserContext()
            : base("Users")
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Connection> Connections { get; set; }
    }

    public class User
    {
        [Key]
        public string UserName { get; set; }
        public ICollection<Connection> Connections { get; set; }
    }

    public class Connection
    {
        public string ConnectionId { get; set; }
        public string UserAgent { get; set; }
        public DateTimeOffset LastActivity { get; set; }

        public string UserName { get; set; }
    }
}
