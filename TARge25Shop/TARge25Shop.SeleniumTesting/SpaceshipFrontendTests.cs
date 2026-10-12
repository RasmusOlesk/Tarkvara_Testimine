using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TARge25Shop.SeleniumTesting
{
    public class SpaceshipFrontendTests
    {
        private const string BaseUrl = "https://localhost:7227/";
        private const string SpaceshipName = "i add name for spaceship";
        private const string SpaceshipType = "i add ship type for spaceship";
        private const string UpdatedSpaceshipName = "updated name for spaceship";
        private const string UpdatedSpaceshipType = "updated ship type for spaceship";

        [Fact]
        public void Should_NavigateToCreate_AddSpaceShip_WithCorrectData_ReturnToIndex()
        {
            IWebDriver driver = SetupAndNavigateSIndex();

            try
            {
                driver.FindElement(By.Id("CreateInIndex")).Click();

                WaitForElement(driver, By.Id("CU_NameEntrySpaceship"));

                InsertSpaceShipData(driver, isUpdate: false);

                driver.FindElement(By.Id("CU_CreateSpaceship")).Click();

                // Wait for the newly created spaceship to appear.
                IWebElement spaceshipRow = WaitForSpaceshipRow(
                    driver, SpaceshipName);

                string actualName = spaceshipRow
                    .FindElement(By.Id("IndexNameSpaceship")).Text;

                string actualType = spaceshipRow
                    .FindElement(By.Id("IndexTypeSpaceship")).Text;

                string actualCrew = spaceshipRow
                    .FindElement(By.Id("IndexCrewSpaceship")).Text;

                Assert.Equal(SpaceshipName, actualName);
                Assert.Equal(SpaceshipType, actualType);
                Assert.Equal("12345", actualCrew);
            }
            finally
            {
                driver.Quit();
            }
        }

        [Fact]
        public void Should_NavigateToDetails_OfASpaceShip_WithPreviouslyCorrectData()
        {
            IWebDriver driver = SetupAndNavigateSIndex();

            try
            {
                IWebElement spaceshipRow = WaitForSpaceshipRow(
                    driver, SpaceshipName);

                spaceshipRow
                    .FindElement(By.Id("IndexSpaceshipDetails"))
                    .Click();

                WaitForElement(driver, By.Id("Details_SpaceshipName"));

                string actualId = driver
                    .FindElement(By.Id("Details_SpaceshipId")).Text;

                string actualName = driver
                    .FindElement(By.Id("Details_SpaceshipName")).Text;

                string actualType = driver
                    .FindElement(By.Id("Details_SpaceshipType")).Text;

                string actualCrew = driver
                    .FindElement(By.Id("Details_SpaceshipCrew")).Text;

                string actualPower = driver
                    .FindElement(By.Id("Details_SpaceshipPower")).Text;

                Assert.False(string.IsNullOrWhiteSpace(actualId));
                Assert.Equal(SpaceshipName, actualName);
                Assert.Equal(SpaceshipType, actualType);
                Assert.Equal("12345", actualCrew);
                Assert.Equal("666667", actualPower);
            }
            finally
            {
                driver.Quit();
            }
        }

        [Fact]
        public void Should_UpdateDetails_OfASpaceShip_WithNewData()
        {
            IWebDriver driver = SetupAndNavigateSIndex();

            try
            {
                IWebElement spaceshipRow = WaitForSpaceshipRow(
                    driver, SpaceshipName);

                spaceshipRow
                    .FindElement(By.Id("IndexSpaceshipUpdate"))
                    .Click();

                WaitForElement(driver, By.Id("CU_NameEntrySpaceship"));

                InsertSpaceShipData(driver, isUpdate: true);

                // Support either button ID used by the CreateUpdate view.
                IWebElement submitButton;

                try
                {
                    submitButton = driver.FindElement(
                        By.Id("CU_UpdateSpaceship"));
                }
                catch (NoSuchElementException)
                {
                    submitButton = driver.FindElement(
                        By.CssSelector(
                            "input[type='submit'], button[type='submit']"));
                }

                submitButton.Click();

                // Wait until the updated spaceship appears on the index.
                IWebElement updatedRow = WaitForSpaceshipRow(
                    driver, UpdatedSpaceshipName);

                string actualName = updatedRow
                    .FindElement(By.Id("IndexNameSpaceship")).Text;

                string actualType = updatedRow
                    .FindElement(By.Id("IndexTypeSpaceship")).Text;

                string actualCrew = updatedRow
                    .FindElement(By.Id("IndexCrewSpaceship")).Text;

                Assert.Equal(UpdatedSpaceshipName, actualName);
                Assert.Equal(UpdatedSpaceshipType, actualType);
                Assert.Equal("12345", actualCrew);
            }
            finally
            {
                driver.Quit();
            }
        }

        private static IWebDriver SetupAndNavigateSIndex()
        {
            string driverDirectory = AppDomain.CurrentDomain.BaseDirectory;

            FirefoxDriverService service =
                FirefoxDriverService.CreateDefaultService(driverDirectory);

            service.DriverServicePath = driverDirectory;
            service.FirefoxBinaryPath = null;

            FirefoxOptions options = new FirefoxOptions();

            IWebDriver driver = new FirefoxDriver(service, options);

            try
            {
                driver.Navigate().GoToUrl(BaseUrl);

                WaitForElement(driver, By.LinkText("Spaceship")).Click();

                WaitForElement(driver, By.Id("IndexTable"));

                return driver;
            }
            catch
            {
                driver.Quit();
                throw;
            }
        }

        private static IWebElement WaitForElement(
            IWebDriver driver,
            By locator)
        {
            WebDriverWait wait = new WebDriverWait(
                driver, TimeSpan.FromSeconds(10));

            return wait.Until(d =>
            {
                IWebElement element = d.FindElement(locator);

                return element.Displayed ? element : null;
            });
        }

        private static IWebElement WaitForSpaceshipRow(
            IWebDriver driver,
            string expectedName)
        {
            WebDriverWait wait = new WebDriverWait(
                driver, TimeSpan.FromSeconds(10));

            return wait.Until(d =>
            {
                IWebElement table = d.FindElement(By.Id("IndexTable"));

                List<IWebElement> rows = table
                    .FindElements(By.CssSelector("tbody tr"))
                    .ToList();

                foreach (IWebElement row in rows)
                {
                    IWebElement nameCell = row.FindElement(
                        By.Id("IndexNameSpaceship"));

                    if (nameCell.Text.Trim() == expectedName)
                    {
                        return row;
                    }
                }

                return null;
            });
        }

        private static void InsertSpaceShipData(
            IWebDriver driver,
            bool isUpdate)
        {
            string name = isUpdate
                ? UpdatedSpaceshipName
                : SpaceshipName;

            string type = isUpdate
                ? UpdatedSpaceshipType
                : SpaceshipType;

            IWebElement nameInput =
                WaitForElement(driver, By.Id("CU_NameEntrySpaceship"));

            nameInput.Clear();
            nameInput.SendKeys(name);

            IWebElement typeInput =
                driver.FindElement(By.Id("CU_ShipTypeEntrySpaceship"));

            typeInput.Clear();
            typeInput.SendKeys(type);

            IWebElement crewInput =
                driver.FindElement(By.Id("CU_CrewEntrySpaceship"));

            crewInput.Clear();
            crewInput.SendKeys("12345");

            IWebElement enginePowerInput =
                driver.FindElement(By.Id("CU_EnginePowerEntrySpaceship"));

            enginePowerInput.Clear();
            enginePowerInput.SendKeys("666667");
        }
    }
}