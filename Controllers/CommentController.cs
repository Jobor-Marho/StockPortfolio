using AutoMapper;
using identity.DTO.Comment;
using identity.interfaces;
using Identity.Extensions;
using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace identity.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IStockRepository _stockRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICommentRepository _commentRepository;

        public CommentController(IStockRepository stockRepository, IMapper mapper, UserManager<AppUser> userManager, ICommentRepository commentRepository)
        {
            _stockRepository = stockRepository;
            _mapper = mapper;
            _userManager = userManager;
            _commentRepository = commentRepository;
        }

        [HttpGet("api/Comments")]
        [SwaggerOperation(
            Summary = "Get all comments",
            Description = "Retrieves all comments from the repository."
        )]
        [SwaggerResponse(200, "Successfully retrieved all comments.", typeof(IEnumerable<Comment>))]
        [SwaggerResponse(404, "No comments found.")]
        public async Task<IActionResult> GetAllComments()
        {
            var comments = await _commentRepository.GetAllCommentsAsync();
            if (!comments.Any())
                return NotFound("No comments found");
            return StatusCode(StatusCodes.Status200OK, comments);
        }

        [HttpPost("api/create/{stockId}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Add a comment for a specific stock",
            Description = "Allows an authenticated user to add a comment for a stock by specifying the stock ID."
        )]
        [SwaggerResponse(201, "Successfully created the comment.", typeof(object))]
        [SwaggerResponse(400, "Bad request, invalid model state.")]
        [SwaggerResponse(401, "Unauthorized, user not authenticated.")]
        [SwaggerResponse(404, "Stock not found.")]
        [SwaggerResponse(500, "Internal server error, failed to create comment.")]
        public async Task<IActionResult> AddComment([FromRoute] int stockId, [FromBody] CommentEntryDTO comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(User.GetUsername());
            if (user == null)
            {
                return Unauthorized(new { message = "User not authorized" }); // 401 is more appropriate than 400
            }

            var stock = await _stockRepository.GetByIdAsync(stockId);
            if (stock == null)
            {
                return NotFound(new { message = "Stock not found" }); // 404 for not found resources
            }

            var result = await _commentRepository.CreateCommentAsync(user.Id, stockId, _mapper.Map<Comment>(comment));
            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Failed to create comment" });
            }

            return CreatedAtAction(
                nameof(GetUserComments), // Or another action that would return the comment
                new { message = "Comment created successfully" });
        }

        [HttpGet("api/Comments/{stockId}")]
        [SwaggerOperation(
            Summary = "Get comments for a specific stock",
            Description = "Retrieves all the comments associated with a particular stock by its ID."
        )]
        [SwaggerResponse(200, "Successfully retrieved the comments.", typeof(List<Comment>))]
        [SwaggerResponse(204, "No comments found for the given stock ID.")]
        [SwaggerResponse(404, "Stock not found.")]
        public async Task<IActionResult> GetCommentsForStock([FromRoute] int stockId)
        {
            var comments = await _commentRepository.GetCommentsForAStockAsync(stockId);
            if (comments.Count < 1)
                return StatusCode(StatusCodes.Status204NoContent, new { message = "No Comment(s) found" });
            return StatusCode(StatusCodes.Status200OK, comments);
        }

        [HttpPut("api/Comment/update/{commentId}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Update a comment",
            Description = "Updates an existing comment identified by the comment ID."
        )]
        [SwaggerResponse(200, "Successfully updated the comment.")]
        [SwaggerResponse(400, "Bad request. The comment was not found.")]
        [SwaggerResponse(401, "Unauthorized. The user is not authenticated.")]
        public async Task<IActionResult> UpdateComment([FromRoute] int commentId, [FromBody] CommentEntryDTO comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(User.GetUsername());
            if (user == null)
            {
                return Unauthorized(new { message = "User not authorized" });
            }

            var existingComment = await _commentRepository.GetCommentByIdAsync(commentId);
            if (existingComment == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "No Comment found" });
            }
            if (existingComment.AppUserId != user.Id)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You are not authorized to update this comment" });
            }

            // Update the comment properties based on the provided data
            // Only update the properties that are not null or empty
            if(string.IsNullOrEmpty(comment.Title) && comment.Content != null)
            {
                existingComment.Content = comment.Content;
            }else if (string.IsNullOrEmpty(comment.Content) && comment.Title != null)
            {
                existingComment.Title = comment.Title;
            }else{
                existingComment.Title = comment.Title;
                existingComment.Content = comment.Content;
            }



            await _commentRepository.UpdateCommentAsync(existingComment);

            return StatusCode(StatusCodes.Status200OK, new { message = "Comment has been updated successfully" });
        }

        [HttpGet("api/Comments/users")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get all comments by the authenticated user",
            Description = "Retrieves all the comments made by the currently authenticated user."
        )]
        [SwaggerResponse(200, "Successfully retrieved the user's comments.", typeof(List<Comment>))]
        [SwaggerResponse(204, "No comments found for the authenticated user.")]
        [SwaggerResponse(401, "Unauthorized user. User is not authenticated or not authorized.")]
        public async Task<IActionResult> GetUserComments()
        {
            // Get the username of the authenticated user
            var username = User.GetUsername();
            if (username == null)
                return Unauthorized(new { message = "User Not Authenticated" });

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { message = "User not authorized" });

            var userComments = await _commentRepository.GetCommentsByUserIdAsync(user.Id);

            if (userComments.Count < 1)
                return StatusCode(StatusCodes.Status204NoContent, new { message = "No Comment(S) found for user" });

            return StatusCode(StatusCodes.Status200OK, userComments);

        }

        [HttpDelete("api/Comment/delete")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Delete a comment",
            Description = "Deletes a specific comment identified by the comment ID."
        )]
        [SwaggerResponse(200, "Successfully deleted the comment.")]
        [SwaggerResponse(400, "The comment was not found.")]
        [SwaggerResponse(401, "Unauthorized. The user is not authenticated.")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(commentId);
            if (comment == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "No Comment(s) found" });
            }
            await _commentRepository.DeleteCommentAsync(comment.Id);
            return StatusCode(StatusCodes.Status200OK, new { message = "Comment has been deleted successfully" });
        }
    }
}