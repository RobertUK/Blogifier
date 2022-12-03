using Blogifier.Core.Providers;
using Blogifier.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Blogifier.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PostController : ControllerBase
	{
		private readonly IPostProvider _postProvider;
        private readonly ICommentProvider _commentProvider;

        public PostController(IPostProvider postProvider, ICommentProvider commentProvider)
		{
			_postProvider = postProvider;
            _commentProvider = commentProvider;
        }

		[HttpGet("list/{filter}/{postType}")]
		public async Task<ActionResult<List<Post>>> GetPosts(PublishedStatus filter, PostType postType)
		{
			return await _postProvider.GetPosts(filter, postType);
		}

		[HttpGet("list/search/{term}")]
		public async Task<ActionResult<List<Post>>> SearchPosts(string term)
		{
			return await _postProvider.SearchPosts(term);
		}

		[HttpGet("byslug/{slug}")]
		public async Task<ActionResult<Post>> GetPostBySlug(string slug)
		{
			return await _postProvider.GetPostBySlug(slug);
		}

		[HttpGet("getslug/{title}")]
		public async Task<ActionResult<string>> GetSlug(string title)
		{
			return await _postProvider.GetSlugFromTitle(title);
		}

		[Authorize]
		[HttpPost("add")]
		public async Task<ActionResult<bool>> AddPost(Post post)
		{
			return await _postProvider.Add(post);
		}

		[Authorize]
		[HttpPut("update")]
		public async Task<ActionResult<bool>> UpdatePost(Post post)
		{
			return await _postProvider.Update(post);
		}

		[Authorize]
		[HttpPut("publish/{id:int}")]
		public async Task<ActionResult<bool>> PublishPost(int id, [FromBody] bool publish)
		{
			return await _postProvider.Publish(id, publish);
		}

		[Authorize]
		[HttpPut("featured/{id:int}")]
		public async Task<ActionResult<bool>> FeaturedPost(int id, [FromBody] bool featured)
		{
			return await _postProvider.Featured(id, featured);
		}

		[Authorize]
		[HttpDelete("{id:int}")]
		public async Task<ActionResult<bool>> RemovePost(int id)
		{
			return await _postProvider.Remove(id);
		}


        // public async Task<ActionResult> Post([FromForm] PersonCreationDTO personCreation)

        // [Route("/post/comment/{postId}")]
        [Route("/post/comment/{postId}")]
        [HttpPost]
        [HttpGet]
        [Authorize("Admin")]
        public async Task<ActionResult<bool>> AddComment( int postId, [FromForm] Comment commentItem)//AddComment(int postId, Comment comment)
        {
            var post = await _postProvider.GetPostById(postId);

            if (!ModelState.IsValid)
            {
                RedirectToPage("Post", post);
            }

            var comment = new Comment();

            comment.IsAdmin = User.Identity.IsAuthenticated;
            comment.Content = commentItem.Content.Trim();
            comment.Author = commentItem.Author.Trim();
            comment.Email = commentItem.Email.Trim();

      
            await _postProvider.AddComment(comment);

            return Redirect(post.GetLink() + "#" + comment.Id);

        }



        [Authorize("Admin")]
        public async Task<ActionResult<bool>> DeleteComment(int postId, int commentId)
        {
            var post = await _postProvider.GetPostById(postId);

            if (post == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            var comment = post.Comments.FirstOrDefault(c => c.Id == commentId);

            if (comment == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            
            await _postProvider.DeleteComment(postId, commentId);
            return Redirect(post.GetLink());

        }
    }
}
