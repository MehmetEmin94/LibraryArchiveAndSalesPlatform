using AutoMapper;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Book;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Note;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Shelf;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;

namespace LibraryArchiveAndSalesPlatform.API.Application
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<CreateShelfDto,Shelf>();
            CreateMap<UpdateShelfDto,Shelf>();
            CreateMap<Shelf, ShelfDto>();

            CreateMap<CreateBookDto, Book>();
            CreateMap<UpdateBookDto, Book>();
            CreateMap<Book, BookDto>();

            CreateMap<CreateNoteDto, Note>();
            CreateMap<UpdateNoteDto, Note>();
            CreateMap<Note, NoteDto>();
        }
    }
}
