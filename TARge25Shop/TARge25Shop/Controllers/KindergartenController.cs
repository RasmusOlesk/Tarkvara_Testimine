using Microsoft.AspNetCore.Mvc;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;
using TARge25Shop.Data;
using TARge25Shop.Models.Kindergarten;
using TARge25SHop.Core.Dto;
using TARge25SHop.Core.ServiceInterface;
using Microsoft.EntityFrameworkCore;

namespace TARge25Shop.Controllers
{
    public class KindergartenController : Controller
    {
        private readonly IKindergartenServices _kindergartenServices;
        private readonly TARge25ShopContext _context;
        private readonly IFileServices _fileService;

        public KindergartenController
            (
                IKindergartenServices kindergartenServices,
                TARge25ShopContext context,
                IFileServices fileService
            )
        {
            _kindergartenServices = kindergartenServices;
            _context = context;
            _fileService = fileService;
        }

        public IActionResult Index()
        {

            // Kutsume teenuse välja, et saada kõik kosmoselaevad. 
            //constructoris tuleb välja kutsuda DbContext, et
            //saaksime andmeid kätte.
            var result = _context.Kindergartens
                .Select(x => new KindergartenIndexViewModel
                {
                    Id = x.Id,
                    GroupName = x.GroupName,
                    KindergartenName = x.KindergartenName,
                    TeacherName = x.TeacherName,
                    CreatedAt = x.CreatedAt,
                    ChildrenCount = x.ChildrenCount
                });

            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            KindergartenCreateUpdateViewModel result = new();

            return View("CreateUpdate", result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(KindergartenCreateUpdateViewModel vm)
        {
            var dto = new KindergartenDto
            {
                GroupName = vm.GroupName,
                KindergartenName = vm.KindergartenName,
                TeacherName = vm.TeacherName,
                ChildrenCount = vm.ChildrenCount,
                Files = vm.Files,
                Image = vm.Image
                    .Select(x => new FileToDatabaseDto
                    {
                        Id = x.ImageId,
                        ImageData = x.ImageData,
                        ImageTitle = x.ImageTitle,
                        KindergartenId = x.KindergartenId
                    }).ToArray()

            };

            //Nüüd kutsume teenuse välja, et luua uus kosmoselaev. See on
            //asünkroonne tegevus ja kasutame await.
            var result = await _kindergartenServices.Create(dto);

            if (result == null)
            {
                // Kui kosmoselaeva loomine ebaõnnestus, siis võime kuvada veateate
                // ja jätta kasutaja samale lehele.
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var kindergarten = await _kindergartenServices.DetailAsync(id);

            if (kindergarten == null)
            {
                return NotFound();
            }

            KindergartenImageViewModel[] images = await FileFromDatabase(id);

            var vm = new KindergartenCreateUpdateViewModel
            {
                Id = kindergarten.Id,
                GroupName = kindergarten.GroupName,
                KindergartenName = kindergarten.KindergartenName,
                TeacherName = kindergarten.TeacherName,
                ChildrenCount = kindergarten.ChildrenCount,
                CreatedAt = kindergarten.CreatedAt,
                UpdatedAt = kindergarten.UpdatedAt,
                Image = images.ToList()
            };

            return View("CreateUpdate", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(KindergartenCreateUpdateViewModel vm)
        {
            var dto = new KindergartenDto()
            {
                Id = (Guid)vm.Id,
                GroupName = vm.GroupName,
                KindergartenName = vm.KindergartenName,
                TeacherName = vm.TeacherName,
                ChildrenCount = vm.ChildrenCount,
                CreatedAt = vm.CreatedAt,
                UpdatedAt = vm.UpdatedAt,
                Files = vm.Files,
                Image = vm.Image
                    .Select(x => new FileToDatabaseDto
                    {
                        Id = x.ImageId,
                        ImageData = x.ImageData,
                        ImageTitle = x.ImageTitle,
                        KindergartenId = x.KindergartenId
                    }).ToArray()
            };

            var result = await _kindergartenServices.Update(dto);

            var kindergartenId = result.Id;

            if (result == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));


        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var kindergarten = await _kindergartenServices.DetailAsync(id);

            if (kindergarten == null)
            {
                return NotFound();
            }

            KindergartenImageViewModel[] images = await FileFromDatabase(id);

            var vm = new KindergartenDeleteViewModel
            {
                Id = kindergarten.Id,
                GroupName = kindergarten.GroupName,
                KindergartenName = kindergarten.KindergartenName,
                TeacherName = kindergarten.TeacherName,
                ChildrenCount = kindergarten.ChildrenCount,
                CreatedAt = kindergarten.CreatedAt,
                UpdatedAt = kindergarten.UpdatedAt,
                Image = images.ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmation(Guid id)
        {
            var kindergarten = await _kindergartenServices.Delete(id);

            if (kindergarten == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var kindergarten = await _kindergartenServices.DetailAsync(id);

            if (kindergarten == null)
            {
                return NotFound();
            }

            KindergartenImageViewModel[] images = await FileFromDatabase(id);

            var vm = new KindergartenDetailsViewModel
            {
                Id = kindergarten.Id,
                GroupName = kindergarten.GroupName,
                KindergartenName = kindergarten.KindergartenName,
                TeacherName = kindergarten.TeacherName,
                ChildrenCount = kindergarten.ChildrenCount,
                CreatedAt = kindergarten.CreatedAt,
                UpdatedAt = kindergarten.UpdatedAt,
                Image = images.ToList()
            };

            return View(vm);
        }

        public async Task<IActionResult> RemoveImage(KindergartenImageViewModel vm)
        {
            var dto = new FileToDatabaseDto()
            {
                Id = vm.ImageId
            };

            var image = await _fileService.RemoveImageFromDatabase(dto);

            var kindergartenId = image.KindergartenId;

            if (image == null)
            {
                return RedirectToAction(nameof(Index));
            }

            //return RedirectToAction(nameof(Index));
            return RedirectToAction(nameof(Update), new { id = kindergartenId });
        }
        //muuta see niimodi, et pärast pildi kustutamist jääks kasutaja
        //samale kinnisvara detailide lehele, mitte ei suunataks tagasi index lehele

        private async Task<KindergartenImageViewModel[]> FileFromDatabase(Guid id)
        {
            return await _context.FileToDatabases
                .Where(x => x.KindergartenId == id)
                .Select(y => new KindergartenImageViewModel
                {
                    ImageId = y.Id,
                    ImageTitle = y.ImageTitle,
                    ImageData = y.ImageData,
                    KindergartenId = y.KindergartenId,
                    Image = string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(y.ImageData))
                }).ToArrayAsync();
        }
    }

}

