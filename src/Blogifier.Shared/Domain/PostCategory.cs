using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Blogifier.Shared
{
    public class PostCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostId { get; set; }
        public Post Post { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
