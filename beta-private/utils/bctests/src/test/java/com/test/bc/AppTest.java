package com.test.bc;

import org.junit.Test;
import org.openqa.selenium.Capabilities;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.openqa.selenium.Capabilities;

/**
 * Unit test for simple App.
 */
public class AppTest 
{
    /**
     * Rigorous Test :-)
     */
    @Test
    public void shouldAnswerWithTrue()
    {
        try
        {
            Capabilities driverCapabilities = DesiredCapabilities.chrome();

            TestParam testParam = new TestParam( driverCapabilities, 1,1, 1, false);
//            TestRunner testRunner = new TestRunner(testParam);
//
//            Thread staticCaller = new Thread(testRunner);
//            staticCaller.start();
//
//            staticCaller.wait(60);

            TestRunner.run(testParam);
            assert true;
        }
        catch (Exception ex)
        {
            System.out.println(ex);
            assert false;
        }
        
    }
}
