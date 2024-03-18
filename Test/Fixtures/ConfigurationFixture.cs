using Microsoft.Extensions.Configuration;

namespace Test.Fixtures;

public class ConfigurationFixture
{
    public IConfiguration Configuration;

    public ConfigurationFixture()
    {
        var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

        Configuration = config;
    }
}
