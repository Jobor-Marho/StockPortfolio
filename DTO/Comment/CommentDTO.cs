namespace Identity.DTO.Comment
{
    public record CommentDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string AppUserId { get; set; } = string.Empty;

        public int StockId { get; set; }
    }
}