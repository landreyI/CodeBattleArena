using CodeBattleArena.Data;
using CodeBattleArena.Models.DBModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeBattleArena.Service.DBServices
{
    public class DBTaskService
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger<DBService> _logger;
        public DBTaskService(AppDBContext dbContext, ILogger<DBService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<int?> AddTaskAsync(TaskProgrammingModel taskProgramming)
        {
            if (taskProgramming == null) return null;
            try
            {
                await _dbContext.TaskProgramming.AddAsync(taskProgramming);
                await _dbContext.SaveChangesAsync();
                return taskProgramming.IdTaskProgramming;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding TaskProgramming to table");
                return null;
            }
        }
        public async Task<int?> AddInputDataAsync(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            try
            {
                InputDataModel inputDataModel = new InputDataModel { Data = data };
                await _dbContext.InputData.AddAsync(inputDataModel);
                await _dbContext.SaveChangesAsync();
                return inputDataModel.IdInputData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding InputData to table");
                return null;
            }
        }
        public async Task<bool> AddTaskInputDataAsync(int? idTask, int? idInputData, string? answer)
        {
            if (idTask == null || idInputData == null) return false;
            try
            {
                TaskInputData taskInputData = new TaskInputData { 
                    TaskProgrammingId = idTask.Value, 
                    InputDataTaskId = idInputData.Value 
                };

                if (answer != null) taskInputData.Answer = answer;

                await _dbContext.TaskInputData.AddAsync(taskInputData);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding InputData to table");
                return false;
            }
        }
        public async Task<List<InputDataModel>> GetInputDataListAsync()
        {
            try
            {
                return await _dbContext.InputData.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when receiving a list of InputData");
                return null;
            }
        }
        public async Task<List<TaskInputData>> GetTaskInputDataByIdTaskAsync(int? idTask)
        {
            if (idTask == null) return null;

            try
            {
                return await _dbContext.TaskInputData
                    .Include(i => i.InputData)
                    .Where(i => i.TaskProgrammingId == idTask)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when receiving a list of InputData");
                return null;
            }
        }
        public async Task<List<TaskProgrammingModel>> GetTaskListAsync()
        {
            try
            {
                return await _dbContext.TaskProgramming.Include(t => t.TaskInputData).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when retrieving the list of TaskProgramming");
                return null;
            }
        }

        public async Task<List<TaskProgrammingModel>> GetTaskListAsync(string lang)
        {
            try
            {
                return await _dbContext.TaskProgramming.Include(t => t.TaskInputData).Where(t => t.Lang == lang).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when retrieving the list of TaskProgramming");
                return null;
            }
        }

        public async Task<TaskProgrammingModel> GetTaskByIdAsync(int? idTask)
        {
            if (idTask == null) return null;
            try
            {
                return await _dbContext.TaskProgramming
                    .Include(t => t.TaskInputData).ThenInclude(ti => ti.InputData)
                    .FirstOrDefaultAsync(t => t.IdTaskProgramming == idTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when retrieving the list of TaskProgramming");
                return null;
            }
        }
        public async Task<List<TaskProgrammingModel>> GetTaskByLangListAsync(string lang)
        {
            if (string.IsNullOrEmpty(lang)) return null;

            try
            {
                return await _dbContext.TaskProgramming.Where(s => s.Lang == lang).Include(t => t.TaskInputData).ThenInclude(ti => ti.InputData).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when retrieving the list of TaskProgramming");
                return null;
            }
        }
        public async Task<bool> SaveEditTaskAsync(TaskProgrammingModel taskProgramming)
        {
            if (taskProgramming == null)
                return false;

            try
            {
                _dbContext.Update(taskProgramming);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when saving changes");
                return false;
            }
        }
        public async Task<bool> DeleteTaskInputDataAsync(int? idTask, int? idInputData)
        {
            if (idTask == null || idInputData == null)
                return false;

            try
            {
                TaskInputData taskInputData = await _dbContext.TaskInputData.FirstOrDefaultAsync(t => t.TaskProgrammingId == idTask && t.InputDataTaskId == idInputData);
                if (taskInputData == null) return false;

                _dbContext.TaskInputData.Remove(taskInputData);

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when saving changes");
                return false;
            }
        }
        public async Task<bool> DeleteTaskAsync(int? idTask)
        {
            if (idTask == null)
                return false;

            try
            {
                var taskProgramming = await _dbContext.TaskProgramming
                    .Include(t => t.Sessions)
                    .Include(t => t.TaskInputData)
                    .FirstOrDefaultAsync(t => t.IdTaskProgramming == idTask);

                if (taskProgramming == null) return false;

                if (taskProgramming.Sessions != null && taskProgramming.Sessions.Count > 0) return false;

                _dbContext.TaskInputData.RemoveRange(taskProgramming.TaskInputData);

                _dbContext.TaskProgramming.Remove(taskProgramming);

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting Task with ID {IdTask}", idTask);
                return false;
            }
        }
    }
}
