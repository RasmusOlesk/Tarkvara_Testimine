using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace TARge25Shop.SeleniumTesting
{
    public class SpaceshipFrontendTests
    {
        [Fact]
        public void Should_NavigateToCreate_AddSpaceShip_WithCorrectData_ReturnToIndex()
        {
            // ülesseade
            IWebDriver driver = SetupAndNavigateSIndex();

            // tegevus
            IWebElement createInIndex = driver.FindElement(By.Id("CreateInIndex"));
            createInIndex.Click();

            // sisestatavad andmed (vaikimisi uued andmed)
            InsertSpaceShipData(driver, isUpdate: false);

            IWebElement cu_CreateSpaceship = driver.FindElement(By.Id("CU_CreateSpaceship"));
            cu_CreateSpaceship.Click();

            // Antud andmebaasile ja brauserile aega salvestada ja reloadida
            Thread.Sleep(3000);

            // andmete kogumine pärast reloadi
            IWebElement indexNameSpaceship = driver.FindElement(By.Id("IndexNameSpaceship"));
            var spaceShipNameData = indexNameSpaceship.Text;
            IWebElement indexTypeSpaceship = driver.FindElement(By.Id("IndexTypeSpaceship"));
            var spaceShipTypeData = indexTypeSpaceship.Text;
            IWebElement indexCrewSpaceship = driver.FindElement(By.Id("IndexCrewSpaceship"));
            var spaceShipCrewData = indexCrewSpaceship.Text;

            // kontroll
            Assert.Equal("i add name for spaceship", spaceShipNameData);
            Assert.Equal("i add ship type for spaceship", spaceShipTypeData);
            Assert.Equal("12345", spaceShipCrewData);

            driver.Quit();
        }

        [Fact]
        public void Should_NavigateToDetails_OfASpaceShip_WithPreviouslyCorrectData()
        {
            // ülesseade
            IWebDriver driver = SetupAndNavigateSIndex();

            var table = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rows = table.FindElements(By.TagName("tr")).ToList();
            if (rows.Count > 0) rows.RemoveAt(0); // Eemaldame päise

            foreach (var row in rows)
            {
                var locatedelement = row.FindElement(By.Id("IndexNameSpaceship"));
                if (locatedelement.Text == "i add name for spaceship")
                {
                    var rowelement = row.FindElement(By.Id("IndexActionsSpaceship"));
                    var button = rowelement.FindElement(By.Id("IndexSpaceshipDetails"));
                    button.Click();
                    Thread.Sleep(1000);
                    break;
                }
            }

            // andmete kogumine pärast reloadi
            IWebElement details_SpaceshipId = driver.FindElement(By.Id("Details_SpaceshipId"));
            var details_id = details_SpaceshipId.Text;

            IWebElement details_SpaceshipName = driver.FindElement(By.Id("Details_SpaceshipName"));
            var details_name = details_SpaceshipName.Text;

            IWebElement details_SpaceshipType = driver.FindElement(By.Id("Details_SpaceshipType"));
            var details_type = details_SpaceshipType.Text;

            IWebElement details_SpaceshipCrew = driver.FindElement(By.Id("Details_SpaceshipCrew"));
            var details_crew = details_SpaceshipCrew.Text;

            IWebElement details_SpaceshipPower = driver.FindElement(By.Id("Details_SpaceshipPower"));
            var details_power = details_SpaceshipPower.Text;

            // kontroll
            Assert.NotNull(details_id);
            Assert.Equal("i add name for spaceship", details_name);
            Assert.Equal("i add ship type for spaceship", details_type);
            Assert.Equal("12345", details_crew);
            Assert.Equal("666667", details_power);

            driver.Quit();
        }

        [Fact]
        public void Should_UpdateDetails_OfASpaceShip_WithNewData()
        {
            // ülesseade
            IWebDriver driver = SetupAndNavigateSIndex();

            var table = driver.FindElement(By.Id("IndexTable"));
            List<IWebElement> rows = table.FindElements(By.TagName("tr")).ToList();
            if (rows.Count > 0) rows.RemoveAt(0);

            foreach (var row in rows)
            {
                var locatedelement = row.FindElement(By.Id("IndexNameSpaceship"));
                if (locatedelement.Text == "i add name for spaceship")
                {
                    var rowelement = row.FindElement(By.Id("IndexActionsSpaceship"));
                    var button = rowelement.FindElement(By.Id("IndexSpaceshipUpdate"));
                    button.Click();
                    Thread.Sleep(1000);
                    break;
                }
            }

            // sisestatavad andmed uuendamiseks (isUpdate: true)
            InsertSpaceShipData(driver, isUpdate: true);

            // PARANDUS SIIN: Kuna Update vaates ei pruugi olla ID-d "CU_CreateSpaceship", 
            // otsime esmalt "CU_UpdateSpaceship" ja kui seda pole, võtame üldise submit nupu.
            IWebElement submitButton;
            try
            {
                submitButton = driver.FindElement(By.Id("CU_UpdateSpaceship"));
            }
            catch (NoSuchElementException)
            {
                // Kui ID-ga ei leitud, otsib lehelt esimest submit tüüpi nuppu
                submitButton = driver.FindElement(By.CssSelector("input[type='submit'], button[type='submit']"));
            }

            submitButton.Click();

            Thread.Sleep(3000);

            driver.Quit();
        }

        private static IWebDriver SetupAndNavigateSIndex()
        {
            string driverDirectory = AppDomain.CurrentDomain.BaseDirectory;

            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(driverDirectory);
            service.DriverServicePath = driverDirectory;
            service.FirefoxBinaryPath = null;

            FirefoxOptions options = new FirefoxOptions();
            IWebDriver driver = new FirefoxDriver(service, options);

            driver.Url = "https://localhost:7227/";

            IWebElement navigateToSpaceship = driver.FindElement(By.LinkText("Spaceship"));
            navigateToSpaceship.Click();

            return driver;
        }

        private void InsertSpaceShipData(IWebDriver driver, bool isUpdate)
        {
            string name = isUpdate ? "updated name for spaceship" : "i add name for spaceship";
            string type = isUpdate ? "updated ship type for spaceship" : "i add ship type for spaceship";

            IWebElement cu_NameEntrySpaceship = driver.FindElement(By.Id("CU_NameEntrySpaceship"));
            cu_NameEntrySpaceship.Clear();
            cu_NameEntrySpaceship.SendKeys(name);

            IWebElement cu_ShipTypeEntrySpaceship = driver.FindElement(By.Id("CU_ShipTypeEntrySpaceship"));
            cu_ShipTypeEntrySpaceship.Clear();
            cu_ShipTypeEntrySpaceship.SendKeys(type);

            IWebElement cu_CrewEntrySpaceship = driver.FindElement(By.Id("CU_CrewEntrySpaceship"));
            cu_CrewEntrySpaceship.Clear();
            cu_CrewEntrySpaceship.SendKeys("12345");

            IWebElement cu_EnginePowerEntrySpaceship = driver.FindElement(By.Id("CU_EnginePowerEntrySpaceship"));
            cu_EnginePowerEntrySpaceship.Clear();
            cu_EnginePowerEntrySpaceship.SendKeys("666667");
        }
    }
}
