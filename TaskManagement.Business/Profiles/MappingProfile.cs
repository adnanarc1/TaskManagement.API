// TaskManagement.API/Profiles/MappingProfile.cs
using AutoMapper;
using TaskManagement.DataAccessLayer.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManagement.Business.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Token, opt => opt.Ignore());

            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.TasksCreated, opt => opt.Ignore())
                .ForMember(dest => dest.TasksAssigned, opt => opt.Ignore());

            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.TasksCreated, opt => opt.Ignore())
                .ForMember(dest => dest.TasksAssigned, opt => opt.Ignore());

            CreateMap<UpdateUserDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Task mappings
            CreateMap<TaskItem, TaskDto>()
                .ForMember(dest => dest.TypeName,
                    opt => opt.MapFrom(src => src.Type != null ? src.Type.Name : null))
                .ForMember(dest => dest.StatusName,
                    opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : null))
                .ForMember(dest => dest.PriorityName,
                    opt => opt.MapFrom(src => src.Priority != null ? src.Priority.Name : null))
                .ForMember(dest => dest.CreatedByName,
                    opt => opt.MapFrom(src => src.Creator != null ?
                        $"{src.Creator.FirstName} {src.Creator.LastName}" : null))
                .ForMember(dest => dest.AssignedToName,
                    opt => opt.MapFrom(src => src.Assignee != null ?
                        $"{src.Assignee.FirstName} {src.Assignee.LastName}" : null));

            CreateMap<CreateTaskDto, TaskItem>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Creator, opt => opt.Ignore())
                .ForMember(dest => dest.Assignee, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Priority, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedOn, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CompletedDate, opt => opt.Ignore());

            CreateMap<UpdateTaskDto, Task>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Master data mappings
            CreateMap<TaskType, TaskTypeDto>()
                .ForMember(dest => dest.ParentName,
                    opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null));

            CreateMap<TaskStatus, TaskStatusDto>();
            CreateMap<Priority, PriorityDto>();
        }
    }
}