using identity.DTO.Comment;
using Identity.Models;

namespace identity.interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetAllCommentsAsync();
        Task<List<Comment>> GetCommentsForAStockAsync(int stockId);
        Task<Comment> GetCommentByIdAsync(int id);
        Task<List<Comment>> GetCommentsByUserIdAsync(string userId);

        Task<bool> CreateCommentAsync(string AppUserID, int stockID, Comment comment);
        Task<bool> UpdateCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(int commentId);
        Task<bool> SaveAsync();
    }
}