package com.test.bc;

import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;

public class TestAddSolution {

    public static void run(TestParam testParam, WebDriver driver)
    {

        Util.click(testParam.getUserId(), driver, By.id("add-new-solution"));
        Util.click(testParam.getUserId(), driver, By.linkText("Start"));
        Util.sendkeys(testParam.getUserId(), driver, By.id("solution.name"), String.format("bc-pp-sol%s", testParam.getUserId()));
        Util.sendkeys(testParam.getUserId(), driver, By.id("solution.description"), String.format("bc-pp-solution%s", testParam.getUserId()));
        Util.sendkeys(testParam.getUserId(), driver, By.id("solution.version"), "1");
        Util.click(testParam.getUserId(), driver, By.cssSelector(".contact:nth-child(3) legend"));

        Util.sendkeys(testParam.getUserId(), driver, By.id("solution.contacts[0].firstName"), String.format("bc-pp-first%s", testParam.getUserId()));
        Util.sendkeys(testParam.getUserId(), driver, By.id("solution.contacts[0].lastName"), String.format("bc-pp-last%s", testParam.getUserId()));
        Util.sendkeys(testParam.getUserId(), driver, By.id("solution.contacts[0].emailAddress"), String.format("bc-pp-user%s@mailinator.com", testParam.getUserId()));
        Util.sendkeys(testParam.getUserId(), driver, By.id("solution.contacts[0].phoneNumber"), String.format("12345 6789 %s", testParam.getUserId()));

        //Util.click(testParam.getUserId(), driver, By.name("action[continue]"));
        //Util.click(testParam.getUserId(), driver, By.cssSelector("#capabilities > .name"));
        //Util.click(testParam.getUserId(), driver, By.cssSelector("#capabilities .unchecked"));
        //Util.click(testParam.getUserId(), driver, By.name("action[continue]"));

    }



}
