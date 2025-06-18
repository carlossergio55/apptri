using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.GitHub
{
    public class Issue
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public IssueState State { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public List<User> Assignees { get; set; } 
        public List<Label> Labels { get; set; }
    }

    public class User
    {
        public string Login { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class Label
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }

    public enum IssueState
    {
        Open,
        Closed,
        All
    }

}
