using Grpc.Core;
using GrpcService.Data;
using GrpcService.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcService.Services
{
	public class ToDoService : ToDoIt.ToDoItBase
	{
		private readonly AppDbContext _db;
		public ToDoService(AppDbContext db) 
		{
			_db = db;
		}

		public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest req, ServerCallContext context)
		{
			if (req.Title == string.Empty || req.Description == string.Empty)
				throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

			var toDoItem = new ToDoItem
			{
				Title = req.Title,
				Description = req.Description,
			};

			await _db.AddAsync(toDoItem);
			await _db.SaveChangesAsync();

			return await Task.FromResult(new CreateToDoResponse
			{
				Id = toDoItem.Id
			});
		}

		public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest req, ServerCallContext context)
		{
			if (req.Id <= 0)
				throw new RpcException(new Status(StatusCode.InvalidArgument, "resource index must be greater than 0"));

			var toDoItem = await _db.ToDoItems.FirstOrDefaultAsync(x => x.Id == req.Id);

			if (toDoItem != null)
			{
				return await Task.FromResult(new ReadToDoResponse
				{
					Id = toDoItem.Id,
					Title = toDoItem.Title,
					Description = toDoItem.Description,
					ToDoStatus = toDoItem.ToDoStatus,
				});
			}

			throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {req.Id}"));
		}

		public override async Task<GetAllResponse> ListToDo(GetAllRequest req, ServerCallContext context)
		{
			var res = new GetAllResponse();
			var toDoItems = await _db.ToDoItems.ToListAsync();

			foreach(var toDo in toDoItems)
			{
				res.ToDo.Add(new ReadToDoResponse
				{
					Id = toDo.Id,
					Title = toDo.Title,
					Description = toDo.Description,
					ToDoStatus = toDo.ToDoStatus,
				});
			}

			return await Task.FromResult(res);
		}

		public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest req, ServerCallContext context)
		{
			if (req.Id <= 0 || req.Title == string.Empty || req.Description == string.Empty)
				throw new RpcException(new Status(StatusCode.InvalidArgument, "resource index must be greater than 0"));

			var toDoItem = await _db.ToDoItems.FirstOrDefaultAsync(x => x.Id == req.Id);

			if (toDoItem == null)
				throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {req.Id}"));
			
			toDoItem.Title = req.Title;
			toDoItem.Description = req.Description;
			toDoItem.ToDoStatus = req.ToDoStatus;

			await _db.SaveChangesAsync();

			return await Task.FromResult(new UpdateToDoResponse
			{
				Id = toDoItem.Id,
			});
		}

		public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest req, ServerCallContext context)
		{
			if (req.Id <= 0)
				throw new RpcException(new Status(StatusCode.InvalidArgument, "resource index must be greater than 0"));

			var toDoItem = _db.ToDoItems.FirstOrDefault(x => x.Id == req.Id);

			if (toDoItem == null)
				throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {req.Id}"));

			_db.Remove(toDoItem);

			await _db.SaveChangesAsync();

			return await Task.FromResult(new DeleteToDoResponse
			{
				Id = toDoItem.Id
			});
		}
	}
}
