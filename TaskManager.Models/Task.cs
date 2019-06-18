using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string DateTime { get; set; }

        public bool? IsDownloadFile { get; set; }
        public bool? IsMoveCatalog { get; set; }
        public bool? IsEmailSend { get; set; }

        public bool? IsSingly { get; set; }

        public bool? IsDaily { get; set; }
        public bool? IsWeekly { get; set; }
        public bool? IsAnnually { get; set; }

        public string Url { get; set; }
        public string WhereDownload { get; set; }
        public string WhatCopyCatalog { get; set; }
        public string WhereCopyCatalog { get; set; }
        public string ToEmail { get; set; }
        public string Thema { get; set; }
        public string Attachment { get; set; }
    }
}
