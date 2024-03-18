using AutoMapper;
using Domain;

namespace Test.Fixtures;

public class MapperFixture
{
    public IMapper Mapper;

    public MapperFixture()
    {
        var config = new MapperConfiguration(options =>
        {
            options.AddProfile(new MappingProfile());
        });

        Mapper = config.CreateMapper();
    }
}
