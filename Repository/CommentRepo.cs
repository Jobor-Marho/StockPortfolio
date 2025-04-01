using identity.DTO.Comment;
using identity.interfaces;
using Identity.Data;
using Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace identity.Repository
{
    public class CommentRepo : ICommentRepository
    {
        private readonly ApplicationDBContext _context;


        public CommentRepo(ApplicationDBContext context)
        {
            _context = context;
        }



        public async Task<bool> CreateCommentAsync(string AppUserID, int stockID, Comment comment)
        {
            var newComment = new Comment
            {
                Title = comment.Title,
                Content = comment.Content,
                StockId = stockID,
                AppUserId = AppUserID
            };

            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == commentId);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Comment>> GetCommentsByUserIdAsync(string userId)
        {
            return await _context.Comments.Where(c => c.AppUserId == userId).ToListAsync();
        }

        public async Task<List<Comment>> GetCommentsForAStockAsync(int stockId)
        {
            return await _context.Comments.Where(c => c.StockId == stockId).ToListAsync();
        }

        public async Task<bool> UpdateCommentAsync(Comment comment)
        {
            var existingComment = _context.Comments.FirstOrDefault(c => c.Id == comment.Id);
            if (existingComment != null)
            {
                existingComment.Title = comment.Title;
                existingComment.Content = comment.Content;
                existingComment.CreatedOn = DateTime.Now;
                await SaveAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}