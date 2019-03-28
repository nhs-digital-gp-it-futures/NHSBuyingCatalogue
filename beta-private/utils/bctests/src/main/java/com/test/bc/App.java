package com.test.bc;

import org.openqa.selenium.remote.RemoteWebDriver;
import org.openqa.selenium.support.ui.WebDriverWait;

import java.net.MalformedURLException;
import java.net.URL;

import org.openqa.selenium.By;
import org.openqa.selenium.Capabilities;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.openqa.selenium.support.ui.ExpectedConditions;

/**
 * Hello world!
 *
 */
public class App 
{
    public static void main( String[] args )
    {
        int cyclesCount = 1;
        int startOrgCount = 2;
        int orgCount = 1;
        int startOrgUserCount = 1;
        int orgUserCount = 4;
        boolean isMultiUser = orgUserCount > 1;
        int threadCount = 0;

        Capabilities driverCapabilities = DesiredCapabilities.chrome();
        //Capabilities driverCapabilities = DesiredCapabilities.firefox();

        TestParam testParam = new TestParam(driverCapabilities,0,0,0, isMultiUser);

        for  (int j = 1; j <= cyclesCount; j++) {
            for (int i = startOrgCount; i < startOrgCount + orgCount ; i++) {
                for (int u = startOrgUserCount; u < startOrgUserCount + orgUserCount; u++) {
                    try {
                        testParam = new TestParam(driverCapabilities, u, i, j, isMultiUser);
                        TestRunner testRunner = new TestRunner(testParam);

                        Thread staticCaller = new Thread(testRunner);
                        staticCaller.start();

                        System.out.println(String.format("Cycle %d. Thread %s started...", j, testParam.getUserId()));

                        Thread.sleep(250);
                        threadCount++;
                    } catch (Exception ex) {
                        Exception newex = new Exception(String.format("Error in cycle %d, thread %s", j, testParam.getUserId()), ex);
                        System.out.println(newex);
                    }
                }
            }
        }
            
        System.out.printf( "Started All %d Threads \n", threadCount);
    }
}
