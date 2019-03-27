package com.test.bc;

import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;

public class Util {

    public static void sendkeys(String id, WebDriver driver, By by, String text)
    {
        WebElement elm = driver.findElement(by);
        System.out.println( String.format("%s: Typed - %s", id, text ));
        elm.sendKeys(text);
    }

    public static void click(String id, WebDriver driver, By by)
    {
        WebElement elm = driver.findElement(by);
        System.out.println(  String.format("%s: Clicked - %s", id, by.toString() ));
        elm.click();
    }
}
