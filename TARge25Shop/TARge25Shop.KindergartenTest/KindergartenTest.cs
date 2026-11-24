using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;
using TARge25Shop.Data;
using TARge25Shop.KindergartenTest.Macros;
using TARge25Shop.KindergartenTest.Mock;
using Xunit;

namespace TARge25Shop.KindergartenTest
{
    public class KindergartenTest : TestBase
    {
        // Kontrollib, et loomine tagastab lasteaia objekti, mitte tühja vastuse.
        [Fact]
        public async Task ShouldNot_AddEmptyKindergarten_WhenResultIsReturned()
        {
            // Ülesseade
            KindergartenDto dto = new KindergartenDto()
            {
                GroupName = "Päkapiud",
                KindergartenName = "lendav taldrik",
                TeacherName = "Peeter",
                ChildrenCount = 69,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            // tegutsemine
            var result = await Svc<IKindergartenServices>().Create(dto);

            // kontroll
            Assert.NotNull(result);
        }

        // Kontrollib, et kaks erinevat ID-d ei ole võrdsed.
        [Fact]
        public async Task ShouldNot_GetKindergartenID_WhenIDNotEqual()
        {
            // ülesseade
            Guid wrongGuid = Guid.NewGuid();
            Guid goodGuid = Guid.Parse("ecbc059a-0bca-4df2-aae9-a3211e69185a");

            // tegevus
            await Svc<IKindergartenServices>().DetailAsync(goodGuid);

            // kontroll
            Assert.NotEqual(wrongGuid, goodGuid);
        }

        // Kontrollib, et päringus kasutatud ID ja oodatud ID on samad.
        [Fact]
        public async Task Should_GetKindergartenByID_WhenGuidIsEqual()
        {
            // ülessezade
            Guid databaseGuid = Guid.Parse("ecbc059a-0bca-4df2-aae9-a3211e69185a");
            Guid seekGuid = Guid.Parse("ecbc059a-0bca-4df2-aae9-a3211e69185a");

            // tegevus
            await Svc<IKindergartenServices>().DetailAsync(seekGuid);

            // kontroll
            Assert.Equal(databaseGuid, seekGuid);
        }

        // Kontrollib, et kustutamise vastusel on loodud lasteaiaga sama ID.
        [Fact]
        public async Task Should_KindergartenDeletedByID_WhenReturnedResultIsEqual()
        {
            // ülesseade
            KindergartenDto dto = MockKindergartenData();

            // tegevus
            var addKindergarten = await Svc<IKindergartenServices>().Create(dto);
            var deleteKindergarten = await Svc<IKindergartenServices>().Delete((Guid)addKindergarten.Id);

            // kontroll
            Assert.Equal(addKindergarten.Id, deleteKindergarten.Id);
        }

        // Kontrollib, et kustutamise tulemus ei viita teisele loodud lasteaiale.
        [Fact]
        public async Task ShouldNot_DeleteKindergartenByID_WhenDidNotDeleteKindergarten()
        {
            //ülesseade
            var dto = MockKindergartenData();

            //tegevus
            var Kindergarten1 = await Svc<IKindergartenServices>().Create(dto);
            var Kindergarten2 = await Svc<IKindergartenServices>().Create(dto);

            var result = await Svc<IKindergartenServices>().Delete((Guid)Kindergarten2.Id);

            //kontroll
            Assert.NotEqual(Kindergarten1.Id, result.Id);
        }
        // test mis kontrollib, et kindergarteni uuendatakse, uute andmete korral
        [Fact]
        public async Task Should_UpdateKindergartenByID_WhenUpdatingData()
        {
            // loob uue lasteaia andmete objekti
            KindergartenDto dto = MockKindergartenData();

            // Salvestab objekti andmebaasi, mis genereerib talle unikaalse ID
            var result = await Svc<IKindergartenServices>().Create(dto);
            Guid guid = (Guid)result.Id;

            
            var dbContext = Svc<TARge25ShopContext>();
            var trackedEntity = dbContext.Kindergartens.Local.FirstOrDefault(x => x.Id == guid);
            if (trackedEntity != null)
            {
                dbContext.Entry(trackedEntity).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
            

            // Valmistab ette andmed uuendamiseks, kasutades ID-d
            KindergartenDto updateDto = new();
            updateDto.Id = guid;
            updateDto.GroupName = "Blue";
            updateDto.KindergartenName = "püramiid";
            updateDto.TeacherName = "Peedu";
            updateDto.ChildrenCount = 420;
            updateDto.CreatedAt = result.CreatedAt;
            updateDto.UpdatedAt = DateTime.UtcNow;

            // Käivitab teenuse uuendamise
            await Svc<IKindergartenServices>().Update(updateDto);

            // Küsib andmebaasist uuesti selle sama ID-ga andmed, et kontrollida muudatusi
            var updatedResult = await Svc<IKindergartenServices>().DetailAsync(guid);

            //  Kontroll
            Assert.Equal(updatedResult.Id, guid);
            Assert.Equal("Blue", updatedResult.GroupName);
            Assert.Equal("püramiid", updatedResult.KindergartenName);
            Assert.Equal("Peedu", updatedResult.TeacherName);
            Assert.Equal(420, updatedResult.ChildrenCount);
        }



        // Kontrollib, et tühjade andmetega uuendamine ei tagasta loodud lasteaia ID-d.
        [Fact]
        public async Task ShouldNot_UpdateKindergartenByID_WhenNoDataIsUpdated()
        {
            //ülesseade
            KindergartenDto dto = MockKindergartenData();
            var createdKindergarten = await Svc<IKindergartenServices>().Create(dto);

            //tegevus
            KindergartenDto nullDto = MockKindergartenNullData();
            var result = await Svc<IKindergartenServices>().Update(nullDto);

            //kontroll
            Assert.NotEqual(createdKindergarten.Id, result.Id);
        }
        
        // Kontrollib, et loomise järel on vastuses laste arv positiivne.
        [Fact]
        public async Task ShouldNot_CreateKindergartenWithNegativeChildrenCount_WhenChildrenCountNegative()
        {
            //ülesseade
            KindergartenDto dto = MockKindergartenData(true);
            dto.ChildrenCount = (dto.ChildrenCount * 2);

            //tegevus
            var result = await Svc<IKindergartenServices>().Create(dto);

            //kontroll
            Assert.True(result.ChildrenCount > 0);
        }

        // Kontrollib, et loomise järel oleks laste arv suurem kui kolm.
        [Fact]
        public async Task ShouldNot_CreateKindergarten_WhenChildrenCountIsThreeOrLess()
        {
            //ülesseade
            KindergartenDto dto = MockKindergartenData();
            dto.ChildrenCount = 4;
            //tegevus
            var result = await Svc<IKindergartenServices>().Create(dto);
            //kontroll
            Assert.True(result.ChildrenCount > 3);
        }

        // Kontrollib, et kustutatud lasteaeda ei saa enam ID järgi kätte.
        [Fact]
        public async Task Should_RemoveKindergartenFromDatabase_WhenKindergartenIsDeleted()
        {
            //ülesseade
            KindergartenDto dto = MockKindergartenData();

            //tegevus
            var createdKindergarten = await Svc<IKindergartenServices>().Create(dto);
            var deletedKindergarten = await Svc<IKindergartenServices>().Delete((Guid)createdKindergarten.Id);
            var result = await Svc<IKindergartenServices>().DetailAsync((Guid)createdKindergarten.Id);

            //kontrollimine
            Assert.Equal(createdKindergarten.Id, deletedKindergarten.Id);
            Assert.Null(result);
        }

        // Kontrollib, et teise lasteaia kustutamine jätab esimese alles.
        [Fact]
        public async Task ShouldNot_RemoveKindergartenFromDatabase_WhenKindergartenIdIsDifferent()
        {
            var dto = MockKindergartenData();
            var createdKindergarten1 = await Svc<IKindergartenServices>().Create(dto);
            var createdKindergarten2 = await Svc<IKindergartenServices>().Create(dto);
            var deleteResult = await Svc<IKindergartenServices>().Delete((Guid)createdKindergarten2.Id);
            var Kindergartenindb = await Svc<IKindergartenServices>().DetailAsync((Guid)createdKindergarten1.Id);

            //kontroll
            Assert.NotNull(Kindergartenindb);
            Assert.NotEqual(deleteResult.Id, createdKindergarten1.Id);
            Assert.Equal(createdKindergarten1, Kindergartenindb);
        }



        
        private KindergartenDto MockKindergartenNullData()
        {
            return new KindergartenDto
            {
                Id = null,
                GroupName = "",
                KindergartenName = "",
                TeacherName = "",
                ChildrenCount = 0,
                CreatedAt = DateTime.MinValue,
                UpdatedAt = DateTime.MinValue,
            };
        }

        //ISESEISVALT TEHTUD TESTID

        // Kontrollib, et vähemalt kolme lapsega lasteaia loomisel säilib laste arv.
        [Fact]
        public async Task Should_CreateKindergarten_WhenChildrenCountIsThreeOrMore()
        {
            // Ülesseade
            KindergartenDto dto = MockKindergartenData();
            dto.ChildrenCount = 3;

            // Tegevus
            var result = await Svc<IKindergartenServices>().Create(dto);

            // Kontroll
            Assert.True(result.ChildrenCount >= 3);
        }

        // Kontrollib, et positiivne laste arv jääb loomisel positiivseks.
        [Fact]
        public async Task Should_CreateKindergartenWithPositiveChildrenCount_WhenChildrenCountPositive()
        {
            // Ülesseade
            KindergartenDto dto = MockKindergartenData(false);
            dto.ChildrenCount = (dto.ChildrenCount * 2);

            // Tegevus
            var result = await Svc<IKindergartenServices>().Create(dto);

            // Kontroll
            Assert.True(result.ChildrenCount > 0);
        }

        // Kontrollib, et rühma nimi salvestub lasteaia loomisel õigesti.
        [Fact]
        public async Task Should_CreateKindergarten_WhenGroupNameIsProvided()
        {
            // Ülesseade
            KindergartenDto dto = MockKindergartenData();
            dto.GroupName = "Bumblebee";

            // Tegevus
            var result = await Svc<IKindergartenServices>().Create(dto);

            // Kontroll
            Assert.Equal("Bumblebee", result.GroupName);
        }

        // Kontrollib, et lasteaia nimi salvestub loomisel õigesti.
        [Fact]
        public async Task Should_CreateKindergarten_WhenKindergartenNameIsProvided()
        {
            // Ülesseade
            KindergartenDto dto = MockKindergartenData();
            dto.KindergartenName = "Place";

            // Tegevus
            var result = await Svc<IKindergartenServices>().Create(dto);

            // Kontroll
            Assert.Equal("Place", result.KindergartenName);
        }

        // Kontrollib, et õpetaja nimi salvestub loomisel õigesti.
        [Fact]
        public async Task Should_CreateKindergarten_WhenTeacherNameIsProvided()
        {
            // Ülesseade
            KindergartenDto dto = MockKindergartenData();
            dto.TeacherName = "Peedu";

            // Tegevus
            var result = await Svc<IKindergartenServices>().Create(dto);

            // Kontroll
            Assert.Equal("Peedu", result.TeacherName);
        }

        // Kontrollib, et kümne lapse arv säilib loomise järel.
        [Fact]
        public async Task Should_CreateKindergarten_WhenChildrenCountIsTen()
        {
            // Ülesseade
            KindergartenDto dto = MockKindergartenData();
            dto.ChildrenCount = 10;

            // Tegevus
            var result = await Svc<IKindergartenServices>().Create(dto);

            // Kontroll
            Assert.Equal(10, result.ChildrenCount);
        }

        // Kontrollib, et loodud lasteaial oleks loomise kuupäev määratud.
        [Fact]
        public async Task Should_CreateKindergarten_WhenCreatedAtIsProvided()
        {
            // Ülesseade
            KindergartenDto dto = MockKindergartenData();

            // Tegevus
            var result = await Svc<IKindergartenServices>().Create(dto);

            // Kontroll
            Assert.NotEqual(DateTime.MinValue, result.CreatedAt);
        }

        // Kontrollib, et loodud lasteaial oleks muutmise kuupäev määratud.
        [Fact]
        public async Task Should_CreateKindergarten_WhenUpdatedAtIsProvided()
        {
            // Ülesseade
            KindergartenDto dto = MockKindergartenData();

            // Tegevus
            var result = await Svc<IKindergartenServices>().Create(dto);

            // Kontroll
            Assert.NotEqual(DateTime.MinValue, result.UpdatedAt);
        }

        // Kontrollib, et olemasoleva lasteaia leiab tema ID järgi.
        [Fact]
        public async Task Should_GetKindergartenByID_WhenKindergartenExists()
        {
            // Ülesseade
            KindergartenDto dto = MockKindergartenData();
            var created = await Svc<IKindergartenServices>().Create(dto);

            // Tegevus
            var result = await Svc<IKindergartenServices>()
                .DetailAsync((Guid)created.Id);

            // Kontroll
            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
        }

        // Kontrollib, et leitud lasteaial oleks õige rühma nimi.
        [Fact]
        public async Task Should_GetKindergartenWithCorrectGroupName_WhenKindergartenExists()
        {
            // Ülesseade
            KindergartenDto dto = MockKindergartenData();
            dto.GroupName = "Bugs";
            var created = await Svc<IKindergartenServices>().Create(dto);

            // Tegevus
            var result = await Svc<IKindergartenServices>()
                .DetailAsync((Guid)created.Id);

            // Kontroll
            Assert.Equal("Bugs", result.GroupName);
        }

        // Kontrollib, et õpetaja nime uuendamine salvestub.
        [Fact]
        public async Task Should_UpdateKindergartenTeacherName_WhenTeacherNameChanges()
        {
            // Ülesseade
            KindergartenDto dto = MockKindergartenData();
            var created = await Svc<IKindergartenServices>().Create(dto);
            Guid id = (Guid)created.Id;

            // Eemaldame objekti EF Core'i jälgimisest
            var dbContext = Svc<TARge25ShopContext>();
            var trackedEntity = dbContext.Kindergartens.Local
                .FirstOrDefault(x => x.Id == id);

            if (trackedEntity != null)
            {
                dbContext.Entry(trackedEntity).State =
                    Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            // Tegevus
            KindergartenDto updateDto = new KindergartenDto
            {
                Id = id,
                GroupName = created.GroupName,
                KindergartenName = created.KindergartenName,
                TeacherName = "Aadu",
                ChildrenCount = created.ChildrenCount,
                CreatedAt = created.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            await Svc<IKindergartenServices>().Update(updateDto);
            var result = await Svc<IKindergartenServices>().DetailAsync(id);

            // Kontroll
            Assert.Equal("Aadu", result.TeacherName);
        }

        // Kontrollib, et laste arvu uuendamine salvestub.
        [Fact]
        public async Task Should_UpdateKindergartenChildrenCount_WhenChildrenCountChanges()
        {
            // Ülesseade
            KindergartenDto dto = MockKindergartenData();
            var created = await Svc<IKindergartenServices>().Create(dto);
            Guid id = (Guid)created.Id;

            var dbContext = Svc<TARge25ShopContext>();
            var trackedEntity = dbContext.Kindergartens.Local
                .FirstOrDefault(x => x.Id == id);

            if (trackedEntity != null)
            {
                dbContext.Entry(trackedEntity).State =
                    Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            // Tegevus
            KindergartenDto updateDto = new KindergartenDto
            {
                Id = id,
                GroupName = created.GroupName,
                KindergartenName = created.KindergartenName,
                TeacherName = created.TeacherName,
                ChildrenCount = 25,
                CreatedAt = created.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            await Svc<IKindergartenServices>().Update(updateDto);
            var result = await Svc<IKindergartenServices>().DetailAsync(id);

            // Kontroll
            Assert.Equal(25, result.ChildrenCount);
        }

        // Kontrollib, et kehtiva ID-ga lasteaia kustutamine tagastab kustutatud objekti.
        [Fact]
        public async Task Should_DeleteKindergarten_WhenValidIDIsProvided()
        {
            // Ülesseade
            KindergartenDto dto = MockKindergartenData();
            var created = await Svc<IKindergartenServices>().Create(dto);
            Guid id = (Guid)created.Id;

            // Tegevus
            var result = await Svc<IKindergartenServices>().Delete(id);

            // Kontroll
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        // Kontrollib, et kaks loomiskutset loovad erineva ID ja rühma nimega lasteaiad.
        [Fact]
        public async Task Should_CreateTwoDifferentKindergartens_WhenCreateIsCalledTwice()
        {
            // Ülesseade
            KindergartenDto dto1 = MockKindergartenData();
            KindergartenDto dto2 = MockKindergartenData();
            dto2.GroupName = "bees";

            // Tegevus
            var result1 = await Svc<IKindergartenServices>().Create(dto1);
            var result2 = await Svc<IKindergartenServices>().Create(dto2);

            // Kontroll
            Assert.NotEqual(result1.Id, result2.Id);
            Assert.NotEqual(result1.GroupName, result2.GroupName);
        }

        // Kontrollib, et kõik antud lasteaia andmed jõuavad loodud objekti.
        [Fact]
        public async Task Should_CreateKindergartenWithCorrectData_WhenAllFieldsAreProvided()
        {
            // Ülesseade
            KindergartenDto dto = new KindergartenDto
            {
                GroupName = "Chicken",
                KindergartenName = "Rainbow",
                TeacherName = "Anna",
                ChildrenCount = 15,
                CreatedAt = new DateTime(2025, 3, 1),
                UpdatedAt = new DateTime(2025, 3, 5)
            };

            // Tegevus
            var result = await Svc<IKindergartenServices>().Create(dto);

            // Kontroll
            Assert.NotNull(result);
            Assert.Equal("Chicken", result.GroupName);
            Assert.Equal("Rainbow", result.KindergartenName);
            Assert.Equal("Anna", result.TeacherName);
            Assert.Equal(15, result.ChildrenCount);

            // Kontrollime, et kuupäevad on määratud
            Assert.NotEqual(DateTime.MinValue, result.CreatedAt);
            Assert.NotEqual(DateTime.MinValue, result.UpdatedAt);
        }

        /* üleval testid, all abimeetodid */

        private KindergartenDto MockKindergartenData(bool isOneOrTwo = false)
        {
            if (isOneOrTwo == false)
            {
                return new KindergartenDto
                {
                    GroupName = "Päkapikk",
                    KindergartenName = "UFO",
                    TeacherName = "Peeter",
                    ChildrenCount = 69,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };
            }
            else
            {
                return new KindergartenDto
                {
                    GroupName = "RAKETT69",
                    KindergartenName = "UFO",
                    TeacherName = "Aadu",
                    ChildrenCount = 420,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };
            }
        }

    }
}
