using AutoMapper;
using identity.DTO.Comment;
using identity.DTO.Stock;
using Identity.DTO.Stock;
using Identity.Models;

namespace identity.Mapper
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Stock, UpdateStockRequestDto>().ReverseMap();
            CreateMap<Comment, CommentEntryDTO>().ReverseMap();
        }
    }
}