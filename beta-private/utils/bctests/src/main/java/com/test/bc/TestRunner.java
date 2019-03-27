package com.test.bc;

import org.openqa.selenium.WebDriverInfo;
import org.openqa.selenium.remote.RemoteWebDriver;
import org.openqa.selenium.support.ui.WebDriverWait;

import java.net.URL;
import java.util.Random;

import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;

/**
 * Test Runner
 *
 */
public class TestRunner implements Runnable
{
    private TestParam _testParam;

    public TestRunner(TestParam testParam)
    {
        _testParam = testParam;
    }

    public static void run(TestParam testParam)
    {

        WebDriver driver = null;

        try {

//            int sleepVal = (int) ((Math.random() * 6000) + 6000) * testParam.Cycle * testParam.ThreadNumber * testParam.UserNumber;
//            System.out.println(String.format("Thread start sleep time: %d", sleepVal));
//            Thread.sleep(sleepVal);
//
            driver = new RemoteWebDriver(new URL("http://localhost:4444/wd/hub"), testParam.DriverCapabilities);
//            driver = new RemoteWebDriver(new URL("http://172.17.0.180:4444/wd/hub"), testParam.DriverCapabilities);
            System.out.println( String.format("Started Run %d", testParam.ThreadNumber));

//            sleepVal = (int) ((Math.random() * 6000) + 6000) * testParam.Cycle;
//            System.out.println(String.format("Thread start sleep time: %d", sleepVal));
//            Thread.sleep(sleepVal);

            // run against chrome
            driver.get("https://preprod.buyingcatalogue.digital.nhs.uk/");
//            driver.get("http://localhost:3000");
            System.out.println(driver.getTitle());

            Util.click(testParam.getUserId(), driver, By.linkText("Support"));
            Util.click(testParam.getUserId(), driver, By.linkText("Log in/Sign up"));

            WebDriverWait wait = new WebDriverWait(driver, 30);
            wait.until(ExpectedConditions.titleContains("Sign In"));
            Thread.sleep(3000);

            Util.sendkeys(testParam.getUserId(), driver, By.name("email"), String.format("bc-pp-user%s@mailinator.com", testParam.getUserId()));
            Util.sendkeys(testParam.getUserId(), driver, By.name("password"), String.format("Password1"));
            Util.click(testParam.getUserId(), driver, By.name("submit"));

            wait = new WebDriverWait(driver, 30);
            wait.until(ExpectedConditions.titleContains("NHS Digital Buying Catalogue"));
            Thread.sleep(3000);

//            WebElement stdcap = driver.findElement(By.linkText("Preprod - JT Sol B | 1"));
//            stdcap.click();

            TestAddSolution.run(testParam, driver);
            Thread.sleep(10000);

            Util.click(testParam.getUserId(), driver, By.linkText("Log Out"));
            Thread.sleep(3000);

        } 
        catch (Exception e) {
                e.printStackTrace();
        }
        finally {

            try{Thread.sleep(10000);}catch(Exception ex){ex.printStackTrace();};

            if (driver != null)
                driver.quit();

        }
        
        System.out.println( String.format("Completed Run %s", testParam.getUserId()));
    }

    @Override
    public void run() {

        run(_testParam);
    }
}
