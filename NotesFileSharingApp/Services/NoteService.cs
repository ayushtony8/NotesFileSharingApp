using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NotesFileSharingApp.DTOs;
using NotesFileSharingApp.Interfaces;
using NotesFileSharingApp.Models;

namespace NotesFileSharingApp.Services
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;
        private readonly ISharedRepository _sharedRepository;
        private readonly IMapper _mapper;

        public NoteService(INoteRepository noteRepository, ISharedRepository sharedRepository, IMapper mapper)
        {
            _noteRepository = noteRepository;
            _sharedRepository = sharedRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NoteDto>> GetAllNotesAsync(string userId)
        {
            var notes = await _noteRepository.GetByUserIdAsync(userId);
            var noteDtos = _mapper.Map<List<NoteDto>>(notes);

            // Check if notes are shared and set properties
            foreach (var noteDto in noteDtos)
            {
                noteDto.IsShared = false; // User's own notes
                noteDto.CanEdit = true; // User can edit their own notes
            }

            return noteDtos;
        }

        public async Task<NoteDto?> GetNoteByIdAsync(int id, string userId)
        {
            var note = await _noteRepository.GetByIdAsync(id);
            if (note == null) return null;

            // Check if user owns the note or has access through sharing
            if (note.UserId == userId)
            {
                var noteDto = _mapper.Map<NoteDto>(note);
                noteDto.IsShared = false;
                noteDto.CanEdit = true;
                return noteDto;
            }

            // Check if note is shared with user
            var sharedNote = await _sharedRepository.GetSharedNoteAsync(id, userId);
            if (sharedNote != null)
            {
                var noteDto = _mapper.Map<NoteDto>(sharedNote.Note);
                noteDto.IsShared = true;
                noteDto.CanEdit = sharedNote.CanEdit;
                return noteDto;
            }

            return null;
        }

        public async Task<NoteDto> CreateNoteAsync(CreateNoteDto createNoteDto, string userId)
        {
            var note = _mapper.Map<Note>(createNoteDto);
            note.UserId = userId;
            
            var createdNote = await _noteRepository.CreateAsync(note);
            var noteDto = _mapper.Map<NoteDto>(createdNote);
            noteDto.IsShared = false;
            noteDto.CanEdit = true;
            
            return noteDto;
        }

        public async Task<NoteDto?> UpdateNoteAsync(UpdateNoteDto updateNoteDto, string userId)
        {
            var existingNote = await _noteRepository.GetByIdAsync(updateNoteDto.Id);
            if (existingNote == null) return null;

            // Check if user owns the note
            if (existingNote.UserId == userId)
            {
                _mapper.Map(updateNoteDto, existingNote);
                var updatedNote = await _noteRepository.UpdateAsync(existingNote);
                var noteDto = _mapper.Map<NoteDto>(updatedNote);
                noteDto.IsShared = false;
                noteDto.CanEdit = true;
                return noteDto;
            }

            // Check if note is shared with edit permission
            var sharedNote = await _sharedRepository.GetSharedNoteAsync(updateNoteDto.Id, userId);
            if (sharedNote?.CanEdit == true)
            {
                _mapper.Map(updateNoteDto, existingNote);
                var updatedNote = await _noteRepository.UpdateAsync(existingNote);
                var noteDto = _mapper.Map<NoteDto>(updatedNote);
                noteDto.IsShared = true;
                noteDto.CanEdit = true;
                return noteDto;
            }

            return null;
        }

        public async Task<bool> DeleteNoteAsync(int id, string userId)
        {
            var canAccess = await UserCanAccessNoteAsync(id, userId);
            if (!canAccess) return false;

            var userOwns = await _noteRepository.UserOwnsNoteAsync(id, userId);
            if (!userOwns) return false; // Only owner can delete

            return await _noteRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<NoteDto>> SearchNotesAsync(string searchTerm, string userId)
        {
            var notes = await _noteRepository.SearchAsync(searchTerm, userId);
            var noteDtos = _mapper.Map<List<NoteDto>>(notes);

            foreach (var noteDto in noteDtos)
            {
                noteDto.IsShared = false;
                noteDto.CanEdit = true;
            }

            return noteDtos;
        }

        public async Task<bool> UserCanAccessNoteAsync(int noteId, string userId)
        {
            // Check if user owns the note
            if (await _noteRepository.UserOwnsNoteAsync(noteId, userId))
                return true;

            // Check if note is shared with user
            return await _sharedRepository.IsNoteSharedWithUserAsync(noteId, userId);
        }
    }
}