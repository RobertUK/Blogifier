using Blogifier.Core.Data;
using Blogifier.Core.Data.Migrations;
using Blogifier.Core.Extensions;
using Blogifier.Shared;
using Blogifier.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ReverseMarkdown.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blogifier.Core.Providers
{
	public interface ICommentProvider
    {

		Task<bool> Remove(int id);


    }

	public class CommentProvider : ICommentProvider
    {
		private readonly AppDbContext _db;
        private readonly ICategoryProvider _categoryProvider;
        private readonly IConfiguration _configuration;

        public CommentProvider(AppDbContext db, ICategoryProvider categoryProvider, IConfiguration configuration)
		{
			_db = db;
            _categoryProvider = categoryProvider;
            _configuration = configuration;
		}

		public async Task<bool> Remove(int id)
		{
			var existing = await _db.Comments.Where(p => p.Id == id).FirstOrDefaultAsync();
			if (existing == null)
				return false;

			_db.Comments.Remove(existing);
			await _db.SaveChangesAsync();
			return true;
		}

		#region Private methods




		#endregion
	}
}
