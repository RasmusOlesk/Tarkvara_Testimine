using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TARge25Shop.SeleniumTesting
{
    //järjesti klass, mis omab omadusi mille järgi teste saab järjestada
    public class OrderAttribute : Attribute
    {
        public int Value { get; } //väärtus
        public OrderAttribute(int value) //lihtne omistuskonstruktor
        {
            Value = value;
        }
    }

    //Xunit järjesti mis implementeerib ITestCaseOrdereri liidesest "OrderTestCases" meetodi, mis on see mis testide käivitamisel
    //järjestab nad vastavalt "Order" omadusele
    public class PriorityOrderer : ITestCaseOrderer
    {
        //meetod võtab sisse kõik test cased
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            //meetod tagastab kõik test cased järjestatuna arendaja defineeritud järjesti järgi.
            return testCases.OrderBy(test =>
            {
                //leitakse omadus                                               omaduse tüüp
                var method = test.TestMethod.Method.GetCustomAttributes(typeof(OrderAttribute)).FirstOrDefault();
                //kui ei leita
                if (method == null)
                {
                    //antakse maksimaalne väärtus, ehk test teostatakse kohe, ega ei
                    //panda teiste pontensiaalselt järjekorras olevate testide vahele
                    return int.MaxValue;
                }
                //kui leitakse
                else
                {
                    //siis tagastatakse omaduse "Order" väärtus
                    return method.GetNamedArgument<int>(nameof(OrderAttribute.Value));
                }
            }).ToList(); //pannakse loendisse, ToList on linq meetod.
        }
    }
    //enne testi klassi kirjeldatakse ära mille järgi testid sorteeritakse
    //tunnuse "TestCaseOrderer"i järgi. Sulgude vahel esimene väärtus on klassi asukoht ise, teine väärtus on nimeruum kus klass asub.
    [TestCaseOrderer("TARge25Shop.SeleniumTesting.PriorityOrderer", "TARge25Shop.SeleniumTesting")]
    public class kindergartenFrontendTests
    {
        public static string thisRunGuid { get; set; }


        [Fact]
        [Order(1)]
        public void Should_NavigateToCreate_AddUniqueKindergarten_WithCorrectData_ReturnToIndex()
        {
            //ülesseade
            IWebDriver driver = SetupAndNavigateSIndex();

            //tegevus
            IWebElement createInIndex = driver.FindElement(By.Id("CreateInIndex"));
            createInIndex.Click();

            //sisestatavad andmed, siin tagastub ka kasutatud nimi
            string concreteName = InsertKindergartenData_ReturnName(driver, false);
            thisRunGuid = concreteName;

            IWebElement cu_CreateKindergarten = driver.FindElement(By.Id("CU_CreateKindergarten"));
            cu_CreateKindergarten.Click();

            ////arvuti tudub
            //Thread.Sleep(1000);

            //andmete kogumine pärast reloadi
            var tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            string kindergartenGroupNameData = "";
            string kindergartenNameData = "";
            string kindergartenTeacherNameData = "";
            string kindergartenCreatedAtData = "";
            //IWebElement indexNameSpaceship = driver.FindElement(By.Id("IndexNameSpaceship"));
            //var spaceShipNameData = indexNameSpaceship.Text;
            //IWebElement indexTypeSpaceship = driver.FindElement(By.Id("IndexTypeSpaceship"));
            //var spaceShipTypeData = indexTypeSpaceship.Text;
            //IWebElement indexCrewSpaceship = driver.FindElement(By.Id("IndexCrewSpaceship"));
            //var spaceShipCrewData = indexCrewSpaceship.Text;
            bool doesItExist = false;
            foreach (var row in rowsOfTable)
            {
                if (row.FindElement(By.Id("IndexNameKindergarten")).Text == thisRunGuid)
                {
                    doesItExist = true;
                    kindergartenGroupNameData = thisRunGuid;
                    kindergartenNameData = row.FindElement(By.Id("IndexNameKindergarten")).Text;
                    kindergartenTeacherNameData = row.FindElement(By.Id("IndexTeacherNameKindergarten")).Text;
                    kindergartenCreatedAtData = row.FindElement(By.Id("IndexCreateDataKindergarten")).Text;
                    break;
                }
            }
            driver.Quit();

            //kontroll
            //Assert.True(concreteName == indexNameSpaceship.Text);
            Assert.True(true == doesItExist);
            Assert.Equal(kindergartenGroupNameData, thisRunGuid);
            Assert.True(kindergartenNameData == "i add name for kindergarten");
            Assert.Equal(kindergartenTeacherNameData, "Peedu");
            Assert.True((thisRunGuid == kindergartenGroupNameData)
                && ("i add name for kindergarten" == kindergartenNameData)
                && ("Peedu" == kindergartenTeacherNameData));
        }


        [Fact]
        [Order(2)]
        public void Should_NavigateToDetails_OfThisKindergarten_WithPreviouslyCorrectData()
        {
            //ülesseade
            IWebDriver driver = SetupAndNavigateSIndex();

            //otsime üles kõik tabeli read, rida htmli tabelis tähistatakse "tr"iga
            //ICollection<IWebElement> table = driver.FindElements(By.TagName("tr"));
            var table = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rows = table.FindElements(By.TagName("tr")).ToList();
            rows.RemoveAt(0);
            bool isObjectFound = false;

            //tsükkel käib kõik read läbi            
            foreach (var row in rows)
            {

                var locatedelement = row.FindElement(By.Id("IndexGroupNamekindergarten"));
                if (locatedelement.Text == thisRunGuid)
                {
                    //siis vajuta selles reas asuvat details nuppu
                    var rowelement = row.FindElement(By.Id("IndexActionsKindergarten"));
                    var button = rowelement.FindElement(By.Id("IndexKindergartenDetails"));
                    button.Click();
                    //pärast õiget vajutust, tsükkel katkestatakse
                    //arvuti tudub
                    Thread.Sleep(500);
                    break;
                }
            }
            //andmete kogumine pärast reloadi
            IWebElement details_KindergartenId = driver.FindElement(By.Id("Details_KindergartenId"));
            var details_id = details_KindergartenId.Text;

            IWebElement details_KindergartenGroupName = driver.FindElement(By.Id("Details_KindergartenGroupName"));
            var details_groupName = details_KindergartenGroupName.Text;

            IWebElement details_KindergartenName = driver.FindElement(By.Id("Details_KindergartenName"));
            var details_name = details_KindergartenName.Text;

            IWebElement details_KindergartenTeacherName = driver.FindElement(By.Id("Details_KindergartenTeacherName"));
            var details_teacherName = details_KindergartenTeacherName.Text;

            IWebElement details_KindergartenChildrenCount = driver.FindElement(By.Id("Details_KindergartenChildrenCount"));
            var details_childrenCount = details_KindergartenChildrenCount.Text;

            driver.Quit();
            //kontroll
            Assert.True(isObjectFound);
            Assert.NotNull(details_id);
            // et ei oleks tühi kontrollid kõikidele andmetele
            Assert.NotEmpty(details_groupName);
            Assert.NotNull(details_name);
            Assert.False(string.IsNullOrEmpty(details_childrenCount));
            Assert.True(!string.IsNullOrEmpty(details_teacherName));
            Assert.Equal(thisRunGuid, details_groupName);
            Assert.Equal("i add name for kindergarten", details_name);
            Assert.Equal("Peedu", details_teacherName);
            Assert.Equal("666667", details_childrenCount);
            Assert.True(int.Parse(details_childrenCount).GetType() == typeof(int));
        }


        [Fact]
        [Order(3)]
        public void Should_UpdateDetails_OfThisKindergarten_WithNewData()
        {
            //ülesseade
            IWebDriver driver = SetupAndNavigateSIndex();

            //otsime üles kõik tabeli read, rida htmli tabelis tähistatakse "tr"iga
            //ICollection<IWebElement> table = driver.FindElements(By.TagName("tr"));
            var table = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rows = table.FindElements(By.TagName("tr")).ToList();
            rows.RemoveAt(0);
            bool doesItExist = false;
            //tsükkel käib kõik read läbi

            foreach (var row in rows)
            {

                var locatedelement = row.FindElement(By.Id("IndexGroupNameKindergarten"));
                if (locatedelement.Text == thisRunGuid)
                {
                    doesItExist = true;
                    //siis vajuta selles reas asuvat details nuppu
                    var rowelement = row.FindElement(By.Id("IndexActionsKindergarten"));
                    var button = rowelement.FindElement(By.Id("IndexKindergartenUpdate"));
                    button.Click();
                    //pärast õiget vajutust, tsükkel katkestatakse
                    //arvuti tudub
                    Thread.Sleep(500);
                    break;
                }
            }
            //sisestatavad andmed
            InsertKindergartenData(driver, thisRunGuid, true);

            IWebElement cu_CreateKindergarten = driver.FindElement(By.Id("CU_UpdateKindergarten"));
            cu_CreateKindergarten.Click();

            //andmete kogumine pärast reloadi
            var tableOfIndex = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rowsOfTable = tableOfIndex.FindElements(By.TagName("tr")).ToList();
            rowsOfTable.RemoveAt(0);
            string kindergartenGroupNameData = "";
            string kindergartenNameData = "";
            string kindergartenTeacherNameData = "";
            string kindergartenCreatedAtData = "";
            doesItExist = false;
            foreach (var row in rowsOfTable)
            {
                if (row.FindElement(By.Id("IndexGroupNameKindergarten")).Text == thisRunGuid)
                {
                    doesItExist = true;
                    kindergartenGroupNameData = thisRunGuid;
                    kindergartenNameData = row.FindElement(By.Id("IndexNameKindergarten")).Text;
                    kindergartenTeacherNameData = row.FindElement(By.Id("IndexTeacherNameKindergarten")).Text;
                    kindergartenCreatedAtData = row.FindElement(By.Id("IndexCreateDateKindergarten")).Text;
                    break;
                }
            }
            driver.Quit();

            ////arvuti tudub
            //Thread.Sleep(1000);
            Assert.True(doesItExist);
            Assert.NotNull(kindergartenGroupNameData);
            Assert.Equal(thisRunGuid, kindergartenGroupNameData);
            Assert.False(string.IsNullOrEmpty(kindergartenNameData));
            Assert.NotEqual(kindergartenNameData, "i add name for kindergarten");
            Assert.True(kindergartenNameData == "completely unheard of ubenuah");
            Assert.False(string.IsNullOrEmpty(kindergartenNameData));
            //kontrolli kõik teised andmed ka üle põhjalikult.
        }


        [Fact]
        [Order(4)]
        public void Should_DeleteThisKindergarten_FromAppVIADeleteView()
        {
            //ülesseade
            IWebDriver driver = SetupAndNavigateSIndex();

            //otsime üles kõik tabeli read, rida htmli tabelis tähistatakse "tr"iga
            //ICollection<IWebElement> table = driver.FindElements(By.TagName("tr"));
            var table = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rows = table.FindElements(By.TagName("tr")).ToList();
            rows.RemoveAt(0);
            bool doesItExist = false;
            //tsükkel käib kõik read läbi

            foreach (var row in rows)
            {

                var locatedelement = row.FindElement(By.Id("IndexGroupNameKindergarten"));
                if (locatedelement.Text == thisRunGuid)
                {
                    doesItExist = true;
                    //siis vajuta selles reas asuvat details nuppu
                    var rowelement = row.FindElement(By.Id("IndexActionsKindergarten"));
                    var button = rowelement.FindElement(By.Id("IndexKindergartenDelete"));
                    button.Click();
                    //pärast õiget vajutust, tsükkel katkestatakse
                    //arvuti tudub
                    Thread.Sleep(500);
                    break;
                }
            }
            var clickthisbutton = driver.FindElement(By.Id("Delete_Delete"));
            clickthisbutton.Click();
            bool isDeleted = false;
            table = driver.FindElement(By.Id("IndexTable"));
            rows = table.FindElements(By.TagName("tr")).ToList();
            rows.RemoveAt(0);
            foreach (var row in rows)
            {
                var nameElement = row.FindElement(By.Id("IndexGroupNameKindergarten")).Text;
                if (nameElement == thisRunGuid)
                {
                    isDeleted = false;
                    break;
                }
                else
                {
                    isDeleted = true;
                }
            }

            Assert.True(isDeleted);
            Assert.False(!isDeleted);
        }


        private static IWebDriver SetupAndNavigateSIndex()
        {
            //firefoxi käskiv ja juhtiv draiver
            IWebDriver driver = new FirefoxDriver();
            //aadress millele draiver navigeerib
            driver.Url = "https://localhost:7227/";
            //lehelt otsitav element
            IWebElement navigateToKindergarten = driver.FindElement(By.LinkText("Kindergarten"));
            //selle elemendiga tehtav tegevus
            navigateToKindergarten.Click();
            return driver;
        }


        private void InsertKindergartenData(IWebDriver driver, string name = null, bool isOneOrTwo = false)
        {

            if (isOneOrTwo)
            {
                IWebElement cu_GroupNameEntryKindergarten = driver.FindElement(By.Id("CU_GroupNameEntryKindergarten"));
                cu_GroupNameEntryKindergarten.Clear();
                if (name != null)
                {
                    cu_GroupNameEntryKindergarten.SendKeys(name);
                }
                else
                {
                    cu_GroupNameEntryKindergarten.SendKeys("DISISENJUNAEM");
                }
                IWebElement cu_NameEntryKindergarten = driver.FindElement(By.Id("CU_NameEntryKindergarten"));
                cu_NameEntryKindergarten.Clear();
                cu_NameEntryKindergarten.SendKeys("completely unheard of haunebu");
                IWebElement cu_ChildrenCountEntryKindergarten = driver.FindElement(By.Id("CU_ChildrenCountEntryKindergarten"));
                cu_ChildrenCountEntryKindergarten.Clear();
                cu_ChildrenCountEntryKindergarten.SendKeys("67890");
                IWebElement cu_TeacherNameEntryKindergarten = driver.FindElement(By.Id("CU_TeacherNameEntryKindergarten"));
                cu_TeacherNameEntryKindergarten.Clear();
                cu_TeacherNameEntryKindergarten.SendKeys("Eedu");
            }
            else
            {
                IWebElement cu_GroupNameEntryKindergarten = driver.FindElement(By.Id("CU_GroupNameEntryKindergarten"));
                cu_GroupNameEntryKindergarten.Clear();
                if (name != null)
                {
                    cu_GroupNameEntryKindergarten.SendKeys(name);
                }
                else
                {
                    cu_GroupNameEntryKindergarten.SendKeys("i add name for kindergarten");
                }
                IWebElement cu_NameEntryKindergarten = driver.FindElement(By.Id("CU_NameEntryKindergarten"));
                cu_NameEntryKindergarten.Clear();
                cu_NameEntryKindergarten.SendKeys("i add name for kindergarten");
                IWebElement cu_ChildrenCountEntryKindergarten = driver.FindElement(By.Id("CU_ChildrenCountEntryKindergarten"));
                cu_ChildrenCountEntryKindergarten.Clear();
                cu_ChildrenCountEntryKindergarten.SendKeys("12345");
                IWebElement cu_TeacherNameEntryKindergarten = driver.FindElement(By.Id("CU_TeacherNameEntryKindergarten"));
                cu_TeacherNameEntryKindergarten.Clear();
                cu_TeacherNameEntryKindergarten.SendKeys("Lätt");
            }
        }

        private string InsertKindergartenData_ReturnName(IWebDriver driver, bool isOneOrTwo = false)
        {
            Guid returnGuid = Guid.NewGuid();
            if (isOneOrTwo)
            {
                IWebElement cu_GroupNameEntryKindergarten = driver.FindElement(By.Id("CU_GroupNameEntryKindergarten"));
                cu_GroupNameEntryKindergarten.Clear();
                cu_GroupNameEntryKindergarten.SendKeys(returnGuid.ToString());
                IWebElement cu_NameEntryKindergarten = driver.FindElement(By.Id("CU_NameEntryKindergarten"));
                cu_NameEntryKindergarten.Clear();
                cu_NameEntryKindergarten.SendKeys("completely unheard of haunebu");
                IWebElement cu_ChildrenCountEntryKindergarten = driver.FindElement(By.Id("CU_ChildrenCountEntryKindergarten"));
                cu_ChildrenCountEntryKindergarten.Clear();
                cu_ChildrenCountEntryKindergarten.SendKeys("67890");
                IWebElement cu_TeacherNameEntryKindergarten = driver.FindElement(By.Id("CU_TeacherNameEntryKindergarten"));
                cu_TeacherNameEntryKindergarten.Clear();
                cu_TeacherNameEntryKindergarten.SendKeys("Eltser");
            }
            else
            {
                IWebElement cu_GroupNameEntryKindergarten = driver.FindElement(By.Id("CU_GroupNameEntryKindergarten"));
                cu_GroupNameEntryKindergarten.Clear();
                cu_GroupNameEntryKindergarten.SendKeys(returnGuid.ToString());
                IWebElement cu_NameEntryKindergarten = driver.FindElement(By.Id("CU_NameEntryKindergarten"));
                cu_NameEntryKindergarten.Clear();
                cu_NameEntryKindergarten.SendKeys("i add name for kindergarten");
                IWebElement cu_ChildrenCountEntryKindergarten = driver.FindElement(By.Id("CU_ChildrenCountEntryKindergarten"));
                cu_ChildrenCountEntryKindergarten.Clear();
                cu_ChildrenCountEntryKindergarten.SendKeys("12345");
                IWebElement cu_TeacherNameEntryKindergarten = driver.FindElement(By.Id("CU_TeacherNameEntryKindergarten"));
                cu_TeacherNameEntryKindergarten.Clear();
                cu_TeacherNameEntryKindergarten.SendKeys("Evin");
            }
            return returnGuid.ToString();
        }
    }
}