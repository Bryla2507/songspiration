using AutoMapper;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;

namespace SongSpiration.BLL.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IMapper _mapper;

        public LikeService(ILikeRepository likeRepository, IMapper mapper)
        {
            _likeRepository = likeRepository;
            _mapper = mapper;
        }

        public async Task Like(Guid pinId, Guid userId)
        {
            var like = new Like
            {
                PinId = pinId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _likeRepository.Add(like);
        }

        public async Task Unlike(Guid pinId, Guid userId)
        {
            var like = await _likeRepository.GetByPinIdAndUserId(pinId, userId);
            if (like != null)
            {
                await _likeRepository.Delete(like);
            }
        }

        public async Task<bool> IsLiked(Guid pinId, Guid userId)
        {
            var like = await _likeRepository.GetByPinIdAndUserId(pinId, userId);
            return like != null;
        }
    }
}