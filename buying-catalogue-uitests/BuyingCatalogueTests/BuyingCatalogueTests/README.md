# Selenium Regression Pack

## Structure of the Suite

The suite is a modified Page Object Model based system that utilises builders and factories to generate the drivers and objects for use in the 
tests. From a high level, this is contained as such:

```
Tests <-- Actions <-- Objects
  ^
  |
Utils
```

Tests are made of various actions, actions use the Objects stored. Utils are included where generic functionality can be leveraged rather than 
repeat in multiple places

### Tests
The tests are written in a semi-business language manner, as shown below:

```
authPage.Login(user);

homePage.AddSolution();
```

### Actions
The actions are the logic flow of the particular user action on the page, as shown in the simple example below:

```
authObjects.Username.SendKeys(user.Username);
authObjects.Password.SendKeys(user.Password);
authObjects.LoginButton.Click();
```

### Objects
The objects contain a user friendly name with an associated method of locating the element on screen, as shown in the example below:

```
[FindsBy(How = How.Id, Using = "username")]
public IWebElement Username { get; set; }
```

This would create the following Selenium code: `driver.FindElement(By.Id("username"))` that can be interacted with in the actions steps

The Objects area is designed to be a `Single Source of Truth`, that is, all objects are defined in this area, and any changes to one of the elements would 
bubble up to each of the places it is used in the actions section

## Setup
1. Restore all packages
2. Ensure the following element is added to the `PATH` environment variable: `C:\Users\{CurrentUser}\.nuget\packages\nunit.consolerunner\3.10.0\tools`
3. Add the following environment variables:
	- CatalogueUrl
	- CatalogueUsers (in the form {username},{password};{username},{password})
4. Build the solution
5. Execute the commands below to run the regression pack

## CLI runner

Use the following command to run all the tests in the selenium regression pack

```nunit3-console [relative path to dll] --result [relative path to results file] -tp browser=[*OPTIONAL*];envData=[*OPTIONAL*]```

### Parameters

The following parameters can be set whilst running in NUnit Console

#### Browser

Add the following parameter to the above command

```-tp browser=chrome```

This will run the tests in a chrome headless browser. The following options are available to use for the browser option:

- chrome (default)
- edge
- ie _note - IE doesn't get past the Auth screen as of writing_

#### EnvData

The `envData` parameter is a relative path to the data file required for the tests to run in a different environment to the default (default is the 
Test environment).

``` -tp envData={path-to-file}.yml```

### Complete CLI command

The below command will run the selenium tests from the command line against the default environment using the Edge browser with a transform applied to 
the result file to give an HTML report

```nunit3-console .\bin\Debug\BuyingCatalogueTests.dll --result "TestResults.html;transform=NunitToHTML.xslt" -tp browser=edge```