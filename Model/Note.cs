using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNotes.Model
{
    public interface HasId
    {
        public string Id { get; set; }
    }
    public class Note: HasId
    {
        public string Id { get; set; }
        public string NotebookId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string FileLocation { get; set; }
    }
}
