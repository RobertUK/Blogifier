using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Blogifier.Shared
{
    public class Comment
    {

        [Required]
        [StringLength(160)]
        public string Author { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; }

        [Required, EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

      
        public int Id { get; set; }

        public bool IsAdmin { get; set; } = false;


      
        public int PostId { get; set; }

        public Post Post { get; set; }
        //[Required]
        //public Post Post { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        [Required]
        public DateTime PubDate { get; set; } = DateTime.UtcNow;

        public string GetGravatar()
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(this.Email.Trim().ToLowerInvariant());
            var hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            for (var i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2", CultureInfo.InvariantCulture));
            }

            return $"https://www.gravatar.com/avatar/{sb.ToString().ToLowerInvariant()}?s=60&d=blank";
        }

        public string RenderContent() => this.Content;

        public int CompareTo(Comment other)
        {
            return Content.ToLower().CompareTo(other.Content.ToLower());
        }


    }
}
