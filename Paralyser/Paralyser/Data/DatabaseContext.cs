using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Paralyser.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("sqlAzure") { }

        public DbSet<ParagraphText> Paragraphs { get; set; }
    }
}