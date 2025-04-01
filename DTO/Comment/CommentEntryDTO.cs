

namespace identity.DTO.Comment
{
    public record CommentEntryDTO
    {
        
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

    }
}