using AutoMapper;
using NotesFileSharingApp.DTOs;
using NotesFileSharingApp.Models;

namespace NotesFileSharingApp.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Note mappings
            CreateMap<Note, NoteDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.IsShared, opt => opt.Ignore())
                .ForMember(dest => dest.CanEdit, opt => opt.Ignore());
            
            CreateMap<CreateNoteDto, Note>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.SharedNotes, opt => opt.Ignore());

            CreateMap<UpdateNoteDto, Note>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.SharedNotes, opt => opt.Ignore());

            // File mappings
            CreateMap<FileUpload, FileUploadDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.IsShared, opt => opt.Ignore());

            // Shared Note mappings
            CreateMap<SharedNote, SharedNoteDto>()
                .ForMember(dest => dest.NoteTitle, opt => opt.MapFrom(src => src.Note.Title))
                .ForMember(dest => dest.NoteContent, opt => opt.MapFrom(src => src.Note.Content))
                .ForMember(dest => dest.SharedByUserName, opt => opt.MapFrom(src => src.SharedByUser.Email))
                .ForMember(dest => dest.SharedWithUserName, opt => opt.MapFrom(src => src.SharedWithUser.Email));

            // Shared File mappings
            CreateMap<SharedFile, SharedFileDto>()
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.File.FileName))
                .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.File.FileType))
                .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.File.FileSize))
                .ForMember(dest => dest.SharedByUserName, opt => opt.MapFrom(src => src.SharedByUser.Email))
                .ForMember(dest => dest.SharedWithUserName, opt => opt.MapFrom(src => src.SharedWithUser.Email));

            // User mappings
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
        }
    }
}