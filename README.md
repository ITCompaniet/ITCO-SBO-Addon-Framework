# SAP Business One Addon Framework
Framework for SAP Business One SDK Addons

[![NuGet version](https://badge.fury.io/nu/ITCO.SboAddon.Framework.svg)](https://badge.fury.io/nu/ITCO.SboAddon.Framework)
[![itcompaniet MyGet Build Status](https://www.myget.org/BuildSource/Badge/itcompaniet?identifier=380b3e3c-7ef1-45a8-8bfa-138cd1536806)](https://www.myget.org/)



## Usage UI API
Create an Windows Application Project and run SboAddonContext() from Main.

Connection to SBO will be made automatically.
```C#
static class Program
{
    [STAThread]
    static void Main()
    {
        Application.Run(new SboAddonContext());
    }
}
```

### Form Controller
MyFormController will automaticly load Forms.MyForm.srf and create an menu item.

```C#
    public class MyFormController : FormController, IFormMenuItem
    {
        public string MenuItemTitle => "My Form";
        public string ParentMenuItemId => SboMenuItem.Finance;
    }
```

## Usage DI API
### Connect
```C#
static class Program
{
    static void Main()
    {
        // From app.config
        SboApp.DiConnectFromAppConfig();
    }
}
```

```XML
  <appSettings>
    <add key="Sbo:ServerName" value="SAP-SERVER"/>
    <add key="Sbo:ServerType" value="MSSQL2014"/>
    <add key="Sbo:CompanyDb" value="SBODemo"/>
    <add key="Sbo:DbUsername" value="sa"/>
    <add key="Sbo:DbPassword" value=""/>
    <add key="Sbo:Username" value="manager"/>
    <add key="Sbo:Password" value=""/>
    <add key="Sbo:LicenceService" value="SAP-SERVER:30000"/>
  </appSettings>
```

### Setup Class

```C#
    public class MySetup : ISetup
    {
        public const string UDT_MyData = "ITCO_Mydata";
        public const string UDF_CardCode = "CardCode";
        public const string UDF_Amount = "Amount";

        public int Version => 1;

        public void Run()
        {
            UserDefinedHelper.CreateTable(UDT_MyData, "My Data")
                .CreateUDF(UDF_CardCode, "CardCode", BoFieldTypes.db_Alpha, 30)
                .CreateUDF(UDF_Amount, "Amount", BoFieldTypes.db_Float, 30, BoFldSubTypes.st_Price);

            SettingService.InitSetting<string>("my.setting.1", "My Setting 1", "value123");
            SettingService.InitSetting<int>("my.setting.2", "My Setting 2", 0);
        }
    }
```

#### Run setup
```C#
// Finds and run all ISetup classes
SetupManager.FindAndRunSetups(GetType().Assembly);
// or run manually
SetupManager.RunSetup(new MySetup());
```

### Settings Service
```C#
var mySetting = SettingService.GetSettingByKey("itco.mysetting", 0);
```

### Recordset Query
```C#
using (var query = new SboRecordsetQuery("SELECT [DocNum] FROM [ORDR]"))
{
    if (query.Count == 0)
        SboApp.Application.MessageBox("No Matches!");

    foreach (var row in query.Result)
    {
        var docNum = row.Item("DocNum").Value;
    }
}
```

### Misc Helpers
```C#
var combobox = Form.GetComboBox("MyCombo");
combobox.AddComboBoxValues("SELECT CardCode, Cardname FROM OCRD");
```
