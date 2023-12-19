using System.Configuration;
using NUnit.Framework;

namespace Swindler.Tests;

[TestFixture]
public class ItWorks
{
    [Test]
    public void Yeah()
    {
        Assert.That(ConfigurationManager.AppSettings["4real"], Is.EqualTo("this values comes from the real app.config"));
        Assert.That(ConfigurationManager.AppSettings["fake"], Is.Null);

        using (AppConfig.Use("Fake.App.config"))
        {
            Assert.That(ConfigurationManager.AppSettings["4real"], Is.Null);
            Assert.That(ConfigurationManager.AppSettings["fake"], Is.EqualTo("this values comes from a fake app.config"));
        }

        Assert.That(ConfigurationManager.AppSettings["4real"], Is.EqualTo("this values comes from the real app.config"));
        Assert.That(ConfigurationManager.AppSettings["fake"], Is.Null);
    }
}