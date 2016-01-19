# SAP Business One Addon Framework
Framework for SAP Business One SDK Addons

[![NuGet version](https://badge.fury.io/nu/ITCO.SboAddon.Framework.svg)](https://badge.fury.io/nu/ITCO.SboAddon.Framework)
[![itcompaniet MyGet Build Status](https://www.myget.org/BuildSource/Badge/itcompaniet?identifier=380b3e3c-7ef1-45a8-8bfa-138cd1536806)](https://www.myget.org/)



## Usage
Create an Windows Application Project and run SboAddonContext() from Main.

Connection to SBO will be made automatically.
```C#
using ITCO.SboAddon.Framework;
using System;
using System.Windows.Forms;

namespace ITCO.OCR
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.Run(new SboAddonContext());
        }
    }
}
```

### Setup Class
```C#
    public class MySetup : ISetup
    {
        public const string UDT_MyData = "ITCO_Mydata";
        public const string UDF_CardCode = "CardCode";
        public const string UDF_Amount = "Amount";

        public int Version
        {
            get { return 1; }
        }

        public void Run()
        {
            UserDefinedHelper.CreateTable(UDT_MyData, "My Data")
                .CreateUDF(UDF_CardCode, "CardCode", BoFieldTypes.db_Alpha, 30)
                .CreateUDF(UDF_Amount, "Amount", BoFieldTypes.db_Float, 30, BoFldSubTypes.st_Price);

            SettingService.InitSetting<string>("my.setting.1", "My Setting 1");
            SettingService.InitSetting<string>("my.setting.2", "My Setting 2");
        }
    }
```

### Form Controller
MyFormController will automaticly load Forms.MyForm.srf and create an menu item.

```C#
    public class MyFormController : FormController, IFormMenuItem
    {
        public string MenuItemTitle
        {
            get { return "My Form"; }
        }

        public string ParentMenuItemId
        {
            get { return SboMenuItem.Finance; }
        }
    }
```
