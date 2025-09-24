using AutoMapper;
using TodoList.Application.Features.Todos.Queries.GetTodos;
using TodoList.Domain.Entities;

namespace TodoList.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TodoItem, TodoDto>()
            .ForMember(d => d.Priority, opt => opt.MapFrom(s => s.Priority.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
    }
}