using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Blogifier.Shared
{
    public class Post
    {
		public Post() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
		public int AuthorId { get; set; }

		public PostType PostType { get; set; }

		[Required]
		[StringLength(160)]
		public string Title { get; set; }
		[Required]
		[StringLength(160)]
		public string Slug { get; set; }
		[Required]
		[StringLength(450)]
		public string Description { get; set; }
		[Required]
		public string Content { get; set; }
		[StringLength(160)]
		public string Cover { get; set; }
		public int PostViews { get; set; }
		public double Rating { get; set; }
		public bool IsFeatured { get; set; }
		public bool Selected { get; set; }

		public DateTime Published { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateUpdated { get; set; }

		public Blog Blog { get; set; }
        public List<PostCategory> PostCategories { get; set; }

        public List<Comment> Comments { get; set; }// = new List<Comment>();

        public string GetLink()
        {
            return $"/{Slug}/";
        }

        public bool AreCommentsOpen(int commentsCloseAfterDays)
        {
            return Published.AddDays(commentsCloseAfterDays) >= DateTime.UtcNow;
        }

        public static string CreateSlug(string title)
        {
            title = title.ToLowerInvariant().Replace(" ", "-");
            title = RemoveDiacritics(title);
            title = RemoveReservedUrlCharacters(title);

            return title.ToLowerInvariant();
        }

        static string RemoveReservedUrlCharacters(string text)
        {
            var reservedCharacters = new List<string>() { "!", "#", "$", "&", "'", "(", ")", "*", ",", "/", ":", ";", "=", "?", "@", "[", "]", "\"", "%", ".", "<", ">", "\\", "^", "_", "'", "{", "}", "|", "~", "`", "+" };

            foreach (var chr in reservedCharacters)
            {
                text = text.Replace(chr, "");
            }

            return text;
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public string RenderContent()
        {
            var result = Content;

            // Set up lazy loading of images/iframes
            result = result.Replace(" src=\"", " src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"");

            // Youtube content embedded using this syntax: [youtube:xyzAbc123]
            var video = "<div class=\"video\"><iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/{0}?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe></div>";
            result = Regex.Replace(result.ToString(), @"\[youtube:(.*?)\]", (Match m) => string.Format(video, m.Groups[1].Value));

            return result.ToString();
        }
    }
}
