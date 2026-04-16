using System;
using System.Collections.Generic;

namespace Accounting.Core.Models
{
    public class JournalEntry
    {
        public int JournalId { get; set; }

        public DateTime EntryDate { get; set; }

        public string ReferenceType { get; set; }

        public int ReferenceId { get; set; }

        public string Description { get; set; }

        public List<JournalLine> Lines { get; set; }
            = new List<JournalLine>();
    }
}
