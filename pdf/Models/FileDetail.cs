namespace pdf.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FileDetail
    {
        public int Id { get; set; }

        [StringLength(60)]
        public string FileName { get; set; }

        public byte[] FileContent { get; set; }
    }
}
