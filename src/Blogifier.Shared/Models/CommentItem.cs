//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Diagnostics.CodeAnalysis;
//using System.Globalization;
//using System.Security.Cryptography;
//using System.Text;

//namespace Blogifier.Shared
//{
//    public class CommentItem : IComparable<CommentItem>
//    {
//        [Required]
//        [StringLength(100)]
//        public string Author { get; set; } = string.Empty;

//        [Required]
//        [StringLength(2000)]
//        public string Content { get; set; } = string.Empty;

//        [Required, EmailAddress]
//        [StringLength(255)]
//        public string Email { get; set; } = string.Empty;

//        [Required]
//        public int CommentId { get; set; }

//        [Required]
//        public int PostId { get; set; }

//        public bool IsAdmin { get; set; } = false;

//        [Required]
//        public DateTime PubDate { get; set; } = DateTime.UtcNow;

//       // public string Gravatatr { get; set; } = string.Empty;

//        public string GetGravatar()
//        {
//            using var md5 = MD5.Create();
//            var inputBytes = Encoding.UTF8.GetBytes(this.Email.Trim().ToLowerInvariant());
//            var hashBytes = md5.ComputeHash(inputBytes);

//            // Convert the byte array to hexadecimal string
//            var sb = new StringBuilder();
//            for (var i = 0; i < hashBytes.Length; i++)
//            {
//                sb.Append(hashBytes[i].ToString("X2", CultureInfo.InvariantCulture));
//            }

//            return $"https://www.gravatar.com/avatar/{sb.ToString().ToLowerInvariant()}?s=60&d=blank";
//        }

//        public string RenderContent() => this.Content;

//        public int CompareTo(CommentItem other)
//        {
//            return Content.ToLower().CompareTo(other.Content.ToLower());
//        }
//    }
//}
