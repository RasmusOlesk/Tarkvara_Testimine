using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TARge25Shop.Core.Domain;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;
using TARge25Shop.Data;
using TARge25SHop.Core.Domain;
using TARge25SHop.Core.Dto;
using TARge25SHop.Core.ServiceInterface;

namespace TARge25Shop.ApplicationServices.Services
{
    public class FileServices : IFileServices
    {
        private readonly IHostEnvironment _webHost;
        private readonly TARge25ShopContext _context;

        public FileServices
            (
                IHostEnvironment webHost,
                TARge25ShopContext context
            )
        {
            _webHost = webHost;
            _context = context;
        }

        public void FilesToApi(SpaceshipDto dto, Spaceship domain)
        {
            if (dto.Files != null && dto.Files.Count > 0)
            {
                if (!Directory.Exists(_webHost.ContentRootPath + "\\wwwroot\\multipleFileUpload\\"))
                {
                    Directory.CreateDirectory(_webHost.ContentRootPath + "\\wwwroot\\multipleFileUpload\\");
                }

                foreach (var file in dto.Files)
                {
                    string uploadsFolder = Path.Combine(_webHost.ContentRootPath, "wwwroot", "multipleFileUpload");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);

                        FileToApi path = new FileToApi
                        {
                            Id = Guid.NewGuid(),
                            ExistingFilePath = uniqueFileName,
                            SpaceshipId = domain.Id
                        };

                        _context.FileToApis.AddAsync(path);
                    }
                }
            }
        }

        public void UploadFilesToDatabase(KindergartenDto dto, Kindergarten domain)
        {
            if (dto.Files != null && dto.Files.Count > 0)
            {
                foreach (var file in dto.Files)
                {
                    using (var target = new MemoryStream())
                    {
                        FileToDatabase files = new FileToDatabase()
                        {
                            Id = Guid.NewGuid(),
                            ImageTitle = file.FileName,
                            KindergartenId = domain.Id
                        };

                        file.CopyTo(target);
                        files.ImageData = target.ToArray();

                        _context.FileToDatabases.AddAsync(files);
                    }
                }
            }
        }

        public async Task<FileToDatabase> RemoveImageFromDatabase(FileToDatabaseDto dto)
        {
            var image = await _context.FileToDatabases
                .Where(x => x.Id == dto.Id)
                .FirstOrDefaultAsync<FileToDatabase>();

            if (image != null)
            {
                _context.FileToDatabases.Remove(image);
                await _context.SaveChangesAsync();
            }

            return image;
        }

        // Massiivi ülekoormus üksiku objekti meetodi kaudu
        public async Task RemoveImageFromDatabase(FileToDatabaseDto[] images)
        {
            await RemoveImagesFromDatabase(images);
        }

        // PARANDUS: Ei tagasta nulli, vaid tagastab viimati kustutatud pildi või teeb tühja töö
        public async Task<FileToDatabase> RemoveImagesFromDatabase(FileToDatabaseDto[] dtos)
        {
            FileToDatabase lastDeletedImage = null;

            foreach (var dto in dtos)
            {
                var image = await _context.FileToDatabases
                    .Where(x => x.Id == dto.Id)
                    .FirstOrDefaultAsync<FileToDatabase>();

                if (image != null)
                {
                    _context.FileToDatabases.Remove(image);
                    lastDeletedImage = image;
                }
            }

            await _context.SaveChangesAsync();
            return lastDeletedImage;
        }

        // Üksiku objekti ülekoormus massiivi meetodi kaudu
        public async Task<FileToDatabase> RemoveImagesFromDatabase(FileToDatabaseDto dto)
        {
            return await RemoveImageFromDatabase(dto);
        }
    }
}
