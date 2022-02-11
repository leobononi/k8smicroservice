using System.Collections.Generic;
using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {
        bool SaveChanges();
        
        IEnumerable<Platform> GetAllPlatforms();
        Platform GetById(int id);
        void CreatePlatform(Platform plat);
    }
}