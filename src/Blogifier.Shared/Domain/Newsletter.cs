using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blogifier.Shared
{
	public class Newsletter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
		public int PostId { get; set; }
		public bool Success { get; set; }

		public DateTime DateCreated { get; set; }
		public DateTime DateUpdated { get; set; }

		public Post Post { get; set; }
	}
}
