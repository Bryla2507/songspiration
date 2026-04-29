using AutoMapper;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;

namespace SongSpiration.BLL.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ICollectionRepository _collectionRepository;
        private readonly IMapper _mapper;

        public CollectionService(ICollectionRepository collectionRepository, IMapper mapper)
        {
            _collectionRepository = collectionRepository;
            _mapper = mapper;
        }

        public async Task<CollectionDto> CreateCollection(CollectionCreateDto collectionDto)
        {
            var collection = _mapper.Map<Collection>(collectionDto);
            collection.Id = Guid.NewGuid();
            collection.CreatedAt = DateTime.UtcNow;
            collection.UpdatedAt = DateTime.UtcNow;

            await _collectionRepository.Add(collection);
            return _mapper.Map<CollectionDto>(collection);
        }

        public async Task<CollectionDto> GetCollectionById(Guid collectionId)
        {
            var collection = await _collectionRepository.GetById(collectionId);
            return _mapper.Map<CollectionDto>(collection);
        }

        public async Task AddPinToCollection(Guid collectionId, Guid pinId)
        {
            await _collectionRepository.AddPinToCollection(collectionId, pinId);
        }

        public async Task RemovePinFromCollection(Guid collectionId, Guid pinId)
        {
            await _collectionRepository.RemovePinFromCollection(collectionId, pinId);
        }
    }
}