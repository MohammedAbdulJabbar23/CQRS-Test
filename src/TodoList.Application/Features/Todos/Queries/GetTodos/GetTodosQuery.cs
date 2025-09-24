
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoList.Application.Common.Models;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Application.Features.Todos.Queries.GetTodos;


public class GetTodosQuery : GetListQuery, IRequest<PagedResult<TodoDto>>
{
    public TodoStatus? Status { get; set; }
    public TodoPriority? Priority { get; set; }
}

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, PagedResult<TodoDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    public GetTodosQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
        _currentUserService = currentUserService;
    }
    public async Task<PagedResult<TodoDto>> Handle(GetTodosQuery request, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;
        var query = _context.TodoItems.AsQueryable();
        if (!request.IncludeDeleted)
        {
            query = query.Where(t => !t.IsDeleted);
        }
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(t =>
                t.Title.Contains(request.SearchTerm) ||
                (t.Description != null && t.Description.Contains(request.SearchTerm)));
        }
        if (request.Status.HasValue)
        {
            query = query.Where(t => t.Status == request.Status.Value);
        }
        if (request.Priority.HasValue)
        {
            query = query.Where(t => t.Priority == request.Priority.Value);
        }
        query = request.SortBy?.ToLower() switch
        {
            "title" => request.SortDescending ?
                query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
            "priority" => request.SortDescending ?
                query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
            "duedate" => request.SortDescending ?
                query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
            "status" => request.SortDescending ?
                query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
            _ => request.SortDescending ?
                query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
        };
        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Where(c => c.UserId == userId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<TodoDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return new PagedResult<TodoDto>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}