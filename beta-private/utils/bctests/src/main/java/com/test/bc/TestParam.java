package com.test.bc;

import org.openqa.selenium.Capabilities;

/**
 * Test Param
 *
 */
public class TestParam
{
    public Capabilities DriverCapabilities;
    public String SearchText;
    public int UserNumber;
    public int ThreadNumber;
    public int Cycle;
    public boolean IsMultiUser;

    public TestParam(Capabilities cap, int userNumber, int threadNumber, int cycle, boolean isMultiUser)
    {
        DriverCapabilities = cap;
        UserNumber = userNumber;
        ThreadNumber = threadNumber;
        Cycle = cycle;
        IsMultiUser = isMultiUser;
    }

    public String getUserId() {
        String retVal = IsMultiUser ? String.format("%d-%d", ThreadNumber, UserNumber) : String.format("%d", ThreadNumber);
        return retVal;
    }
}