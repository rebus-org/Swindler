# Swindler

Small utility that can trick `ConfigurationManager` into loading its configuration from an `App.config` that YOU specify.

Can be useful for testing units that are sprinkled with 

    var dis = ConfigurationManager.AppSetting["key"];

and

    var dat = ConfigurationManager.ConnectionStrings["name"];

## How to use it

You can use it like this:

    using (AppConfig.Use("Fake.App.config"))
    {
        var appSetting = ConfigurationManager.AppSettings["fake"];
        
        Console.WriteLine($"This is from the fake App.config: {appSetting}");
    }

Remember to dispose it after use.

## How to get started

    Install-Package Swindler -ProjectName <your-project>

