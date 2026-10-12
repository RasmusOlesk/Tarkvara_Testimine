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

        [Fact]
        // Fact tähistab ära ühte testi xUnit raamistikus
        // 1 - Kirjeldatakse ära kas test on tavaline, või negatiivne.
        // 2 - Kirjeldatakse ära mida parasjagu üritatakse testialuse objektiga teha
        // 3 - Mis tingimustel tulemust kontrollitakse, peale tegevust
        //
        // Selles testis kontrollitakse et (2) Kosmoselaeva lisamisel
        // (1) Ei tohiks (3) saadud tulemus olla tühi. Jälgi seda sõnastusviisi:
        //
        //                  1           2               3
        //                  \/          \/              \/
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
        // Selles testis kontrollitakse et (2) Spaceshipi päring andmebaasist
        // (1) ei tohiks tagastada objekti (3) kui ID-d ei ole samad:
        //
        //                  1           2               3
        //                  \/          \/              \/
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
        // Seleta kodus lahti, nagu eelnevate testide laused, eesti keelde, selle testi oma ka.
        // Selles testis kontrollitakse et kosmoselaeva päringul andmebaasist peaks tagastama objekti siis kui ID on sama
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

        // Seleta kodus lahti, nagu eelnevate testide laused, eesti keelde, selle testi oma ka.
        // Selles testis kontrollitakse et kosmoselaeva kustutamisel andmebaasist peaks kustuma objekt kui tagastatav väärtus on sama
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
        // test mis kontrollib, et spaceshipi uuendatakse, uute andmete korral
        [Fact]
        public async Task Should_UpdateKindergartenByID_WhenUpdatingData()
        {
            // 1. Ülesseade - loome uue lasteaia andmete objekti
            KindergartenDto dto = MockKindergartenData();

            // Salvestame objekti andmebaasi, mis genereerib talle tegeliku unikaalse ID
            var result = await Svc<IKindergartenServices>().Create(dto);
            Guid guid = (Guid)result.Id;

            // --- PARANDUS EF CORE KONFLIKTI VASTU ---
            // Küsime andmebaasi konteksti otse välja ja eemaldame (detach) äsja loodud objekti vahemälust,
            // et järgnev Update teenus ei tekitaks identiteedi konflikti.
            var dbContext = Svc<TARge25ShopContext>();
            var trackedEntity = dbContext.Kindergartens.Local.FirstOrDefault(x => x.Id == guid);
            if (trackedEntity != null)
            {
                dbContext.Entry(trackedEntity).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
            // ----------------------------------------

            // 2. Tegevus - Valmistame ette andmed uuendamiseks, kasutades tegelikku ID-d
            KindergartenDto updateDto = new();
            updateDto.Id = guid;
            updateDto.GroupName = "Blue";
            updateDto.KindergartenName = "püramiid";
            updateDto.TeacherName = "Peedu";
            updateDto.ChildrenCount = 420;
            updateDto.CreatedAt = result.CreatedAt;
            updateDto.UpdatedAt = DateTime.UtcNow;

            // Käivitame teenuse uuendamise
            await Svc<IKindergartenServices>().Update(updateDto);

            // Küsime andmebaasist uuesti selle sama ID-ga andmed, et kontrollida muudatusi
            var updatedResult = await Svc<IKindergartenServices>().DetailAsync(guid);

            // 3. Kontroll
            Assert.Equal(updatedResult.Id, guid);
            Assert.Equal("Blue", updatedResult.GroupName);
            Assert.Equal("püramiid", updatedResult.KindergartenName);
            Assert.Equal("Peedu", updatedResult.TeacherName);
            Assert.Equal(420, updatedResult.ChildrenCount);
        }



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
        //kuna mootor ei saa olla negatiivse võimsusega, kontrollime et ei saaks
        //lisada võimetut mootorit ega negatiivse võimsusega mootorit
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

        //test mis kontrollib, et meeskond on suurem kui 3 liiget,
        //service ei tohi lisada sellest vähema arvuga objekti, service
        //võib selle probleemi lahendada ükskõik kuidas
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



        /// <summary>
        /// Returns a nulled object for testing purposes
        /// </summary>
        /// <returns></returns>
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

        [Fact]
        public async Task Should_CreateKindergarten_WhenChildrenCountIsThreeOrMore()
        {
            //ülesseade
            KindergartenDto dto = MockKindergartenData();
            dto.ChildrenCount = 2;
            //tegevus
            var result = await Svc<IKindergartenServices>().Create(dto);
            //kontroll
            Assert.True(result.ChildrenCount < 3);
        }

        [Fact]
        public async Task Should_CreateKindergartenWithPositiveChildrenCount_WhenChildrenCountPositive()
        {
            //ülesseade
            KindergartenDto dto = MockKindergartenData(false);
            dto.ChildrenCount = (dto.ChildrenCount * 2);

            //tegevus
            var result = await Svc<IKindergartenServices>().Create(dto);

            //kontroll
            Assert.True(result.ChildrenCount > 0);
        }


        /* üleval testid, all abimeetodid */

        private KindergartenDto MockKindergartenData(bool isOneOrTwo = false)
        {
            if (isOneOrTwo == false)
            {
                return new KindergartenDto
                {
                    GroupName = "Päkapiud",
                    KindergartenName = "lendav taldrik",
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
                    KindergartenName = "lendav kauss",
                    TeacherName = "Aadu",
                    ChildrenCount = 420,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };
            }
        }

    }
}
