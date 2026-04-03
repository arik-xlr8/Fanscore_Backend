using Fanscore.Application.DTOs.User;

namespace Fanscore.Application.Interfaces.Services
{
    public interface IProfileService
    {
        Task<ProfileDto> GetMyProfileAsync(int userId);
        Task<ProfileDto> UpdateMyProfileAsync(int userId, UpdateProfileDto dto);
        Task<UploadProfilePhotoResultDto> UploadProfilePhotoAsync(
            int userId,
            Stream fileStream,
            string originalFileName,
            string webRootPath,
            string baseUrl
        );
    }
}