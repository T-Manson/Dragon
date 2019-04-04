using AutoMapper;
using Dragon.Samples.WepApi.DBModels;
using Dragon.Samples.WepApi.DomainModels;


namespace Dragon.Samples.WepApi.MappingConfigs
{
    /// <summary>
    /// ResponseMapping映射配置
    /// <summary>
    public class ResponseMapping : Profile
    {
        public ResponseMapping()
        {
            // 字段映射配置
            CreateMap<MyTest, MyTestResponse>().ForMember(target => target.NameTest, opt => opt.MapFrom(source => source.Name));
        }
    }
}