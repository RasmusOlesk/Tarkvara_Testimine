using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TARge25Shop.Core.Domain;
using TARge25Shop.Core.Dto;
using TARge25SHop.Core.Domain;
using TARge25SHop.Core.Dto;

namespace TARge25SHop.Core.ServiceInterface
{
    public interface IFileServices
    {

        void UploadFilesToDatabase(KindergartenDto dto, Kindergarten domain);

        Task<FileToDatabase> RemoveImageFromDatabase(FileToDatabaseDto dto);
        Task<FileToDatabase> RemoveImagesFromDatabase(FileToDatabaseDto dto);
        Task RemoveImageFromDatabase(FileToDatabaseDto[] images);
        void FilesToApi(SpaceshipDto dto, Spaceship spaceShip);
    }
}
