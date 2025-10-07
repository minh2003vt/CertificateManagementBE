using System.IO;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(Stream imageStream, string fileName);
    }
}

