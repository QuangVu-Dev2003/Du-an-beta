using API_Powered_Hospital_Delivery_Robot.Models.DTOs;
using API_Powered_Hospital_Delivery_Robot.Models.Entities;
using API_Powered_Hospital_Delivery_Robot.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace API_Powered_Hospital_Delivery_Robot.Repositories.ImplRepository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly RobotManagerContext _context;

        public TaskRepository(RobotManagerContext context)
        {
            _context = context;
        }

        public async Task<bool> CancelAsync(ulong id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return false;
            }

            task.Status = "canceled";
            task.CompletedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Models.Entities.Task> CreateAsync(Models.Entities.Task task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskStop> CreateTaskStopAsync(TaskStop taskStop)
        {
            _context.TaskStops.Add(taskStop);
            await _context.SaveChangesAsync();
            return taskStop;
        }

        public async Task<IEnumerable<Models.Entities.Task>> GetAllAsync(string? priority = null)
        {
            var query = _context.Tasks.Include(t => t.Robot).Include(t => t.AssignedByNavigation).Include(t => t.TaskStops).AsQueryable();
            if (!string.IsNullOrEmpty(priority))
            {
                query = query.Where(t => t.Priority.ToLower() == priority.ToLower());
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Models.Entities.Task>> GetByAssignedByAsync(ulong assignedById)
        {
            return await _context.Tasks
                .Where(t => t.AssignedBy == assignedById)
                .Include(t => t.Robot)
                .Include(t => t.AssignedByNavigation)
                .Include(t => t.TaskStops)
                .ToListAsync();
        }

        public async Task<Models.Entities.Task?> GetByIdAsync(ulong id)
        {
            return await _context.Tasks
                 .Include(t => t.Robot)
                 .Include(t => t.AssignedByNavigation)
                 .Include(t => t.TaskStops)
                 .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TaskStop>> GetTaskStopsByTaskIdAsync(ulong taskId)
        {
            return await _context.TaskStops
                .Include(ts => ts.Destination)
                .Where(ts => ts.TaskId == taskId)
                .OrderBy(ts => ts.SeqNo)
                .ToListAsync();
        }

        public async Task<Models.Entities.Task?> UpdateAsync(ulong id, Models.Entities.Task task)
        {
            var existing = await _context.Tasks.FindAsync(id);
            if (existing == null)
            {
                return null;
            }

            existing.RobotId = task.RobotId;
            existing.AssignedBy = task.AssignedBy;
            existing.Status = task.Status;
            existing.Priority = task.Priority;
            existing.StartedAt = task.StartedAt;
            existing.CompletedAt = task.CompletedAt;
            existing.TotalDurationS = task.TotalDurationS;
            existing.TotalErrors = task.TotalErrors;
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<Models.Entities.Task?> UpdateTaskProgressAsync(ulong taskId, int seqNo, string stopStatus, int? durationS = null)
        {
            var task = await _context.Tasks.Include(t => t.TaskStops).FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
            {
                return null;
            }

            var stop = task.TaskStops.FirstOrDefault(s => s.SeqNo == seqNo);
            if (stop == null)
            {
                return null;
            }

            stop.Status = stopStatus;
            if (stopStatus == "delivered")
            {
                stop.HandedOverAt = DateTime.UtcNow;
            }
            stop.UpdatedAt = DateTime.UtcNow;

            // Tính progress task
            var completedStops = task.TaskStops.Count(s => s.Status == "delivered" || s.Status == "skipped");
            var progress = (completedStops / (double)task.TaskStops.Count) * 100;
            var robot = await _context.Robots.FirstOrDefaultAsync(r => r.Id == task.RobotId);
            if (robot != null)
            {
                robot.ProgressOverallPct = (decimal)progress;
                _context.Robots.Update(robot);
            }

            // Nếu all stops completed, complete task
            if (task.TaskStops.All(s => s.Status == "delivered" || s.Status == "skipped"))
            {
                task.Status = "completed";
                task.CompletedAt = DateTime.UtcNow;
                task.TotalDurationS = durationS ?? 0;
            }
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return task;
        }
    }
}
