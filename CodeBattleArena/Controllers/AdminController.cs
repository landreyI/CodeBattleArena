using CodeBattleArena.Models;
using CodeBattleArena.Models.DBModels;
using CodeBattleArena.Models.ViewModel;
using CodeBattleArena.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CodeBattleArena.Controllers
{
    public class AnswerItem
    {
        public string InputData { get; set; }
        public string Answer { get; set; }
    }

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DBService dbService;
        public AdminController(IWebHostEnvironment webHostEnvironment, DBService _bdService)
        {
            _webHostEnvironment = webHostEnvironment;
            dbService = _bdService;
        }

        [HttpGet]
        public IActionResult AddPhotoAvatar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoAvatar(IFormFile photo)
        {
            if (ModelState.IsValid)
            {
                // Путь к папке, куда будет сохранено фото
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/avatar");

                // Генерация уникального имени для файла
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(photo.FileName);

                // Полный путь к файлу
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Создание директории, если она не существует
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Сохранение файла в папку img/avatar
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }

                return RedirectToAction("HomePage", "Home");
            }

            return View(photo);
        }

        [HttpGet]
        public async Task<IActionResult> AddTask()
        {
            ViewBag.InputDataTask = await dbService.TaskService.GetInputDataListAsync();
            return View(new TaskModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddTask(TaskModel taskModel, string? inputDataAddIdJson, string? inputDataCreateJson)
        {
            if (ModelState.IsValid)
            {
                var inputDataAddId = JsonConvert.DeserializeObject<List<AnswerItem>>(inputDataAddIdJson);
                var inputDataCreate = JsonConvert.DeserializeObject<List<AnswerItem>>(inputDataCreateJson);

                TaskProgrammingModel model = new TaskProgrammingModel {
                    Name = taskModel.NameTask,
                    Lang = taskModel.LangProgramming,
                    Difficulty = taskModel.Difficulty,
                    TextTask = taskModel.TextTask,
                    Preparation = taskModel.Preparation,
                    VerificationCode = taskModel.VerificationCode,
                };

                int? idTask = await dbService.TaskService.AddTaskAsync(model);

                if (idTask == null) 
                {
                    ViewBag.ErrorMessage = "An error occurred while creating the task!";
                    return View(taskModel);
                }
                
                if(inputDataCreate != null && inputDataCreate.Count != 0) 
                {
                    foreach(var inputData in inputDataCreate)
                    {
                        int? idInputData = await dbService.TaskService.AddInputDataAsync(inputData.InputData);

                        if (idInputData == null)
                        {
                            ViewBag.ErrorMessage = "An error occurred while creating the input data!";
                            return View(taskModel);
                        }

                        if(!await dbService.TaskService.AddTaskInputDataAsync(idTask, idInputData, inputData.Answer))
                        {
                            ViewBag.ErrorMessage = "An error occurred while connecting input data to the job!";
                            return View(taskModel);
                        }
                    }

                }

                if(inputDataAddId != null && inputDataAddId.Count != 0)
                {
                    foreach(var idInputData in inputDataAddId)
                    {
                        if (!await dbService.TaskService.AddTaskInputDataAsync(idTask, int.Parse(idInputData.InputData), idInputData.Answer))
                        {
                            ViewBag.ErrorMessage = "An error occurred while connecting input data to the job!";
                            return View(taskModel);
                        }
                    }
                }
                
                return RedirectToAction("HomePage", "Home");
            }

            ViewBag.InputDataTask = await dbService.TaskService.GetInputDataListAsync();
            return View(taskModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditTask(int? idTask)
        {
            ViewBag.InputDataTask = await dbService.TaskService.GetInputDataListAsync();

            TaskProgrammingModel model = await dbService.TaskService.GetTaskByIdAsync(idTask);

            TaskModel taskModel = new TaskModel()
            {
                IdTask = model.IdTaskProgramming,
                NameTask = model.Name,
                LangProgramming = model.Lang,
                TextTask = model.TextTask,
                Preparation = model.Preparation,
                VerificationCode = model.VerificationCode
            };

            foreach(var taskIputData in model.TaskInputData)
            {
                taskModel.taskInputDataList.Add(taskIputData);
            }

            return View(taskModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditTask(TaskModel taskModel, string? inputDataAddIdJson, string? inputDataCreateJson)
        {
            if (ModelState.IsValid)
            {
                var inputDataAddId = JsonConvert.DeserializeObject<List<AnswerItem>>(inputDataAddIdJson);
                var inputDataCreate = JsonConvert.DeserializeObject<List<AnswerItem>>(inputDataCreateJson);

                TaskProgrammingModel model = new TaskProgrammingModel
                {
                    IdTaskProgramming = taskModel.IdTask.Value,
                    Name = taskModel.NameTask,
                    Lang = taskModel.LangProgramming,
                    Difficulty = taskModel.Difficulty,
                    TextTask = taskModel.TextTask,
                    Preparation = taskModel.Preparation,
                    VerificationCode = taskModel.VerificationCode
                };

                int? idTask = model.IdTaskProgramming;

                if (idTask == null)
                {
                    ViewBag.ErrorMessage = "An error occurred while edit the task!";
                    return View(taskModel);
                }

                if(!await dbService.TaskService.SaveEditTaskAsync(model))
                {
                    ViewBag.ErrorMessage = "An error occurred while edit the task!";
                    return View(taskModel);
                }
                
                if (inputDataCreate != null && inputDataCreate.Count != 0)
                {
                    foreach (var inputData in inputDataCreate)
                    {
                        int? idInputData = await dbService.TaskService.AddInputDataAsync(inputData.InputData);

                        if (idInputData == null)
                        {
                            ViewBag.ErrorMessage = "An error occurred while creating the input data!";
                            return View(taskModel);
                        }

                        if (!await dbService.TaskService.AddTaskInputDataAsync(idTask, idInputData, inputData.Answer))
                        {
                            ViewBag.ErrorMessage = "An error occurred while connecting input data to the job!";
                            return View(taskModel);
                        }
                    }

                }

                if (inputDataAddId != null && inputDataAddId.Count != 0)
                {
                    TaskProgrammingModel taskProgramming = await dbService.TaskService.GetTaskByIdAsync(idTask);

                    foreach (var InputData in taskProgramming.TaskInputData)
                    {
                        if (inputDataAddId.Any(idInputData => int.Parse(idInputData.InputData) == InputData.InputDataTaskId)) 
                        {
                            
                        }
                        else
                        {
                            await dbService.TaskService.DeleteTaskInputDataAsync(idTask, InputData.InputDataTaskId);
                        }
                        
                    }

                    foreach (var idInputData in inputDataAddId)
                    {
                        if(taskProgramming.TaskInputData.Any(inputData => inputData.InputDataTaskId == int.Parse(idInputData.InputData))) 
                        {

                        }
                        else
                        {
                            if (!await dbService.TaskService.AddTaskInputDataAsync(idTask, int.Parse(idInputData.InputData), idInputData.Answer))
                            {
                                ViewBag.ErrorMessage = "An error occurred while connecting input data to the job!";
                                return View(taskModel);
                            }
                        }
                    }
                }

                return RedirectToAction("EditTask", "Admin", new { idTask = idTask });
            }

            ViewBag.InputDataTask = await dbService.TaskService.GetInputDataListAsync();
            return View(taskModel);
        }
        
        [HttpGet]
        public async Task<IActionResult> DeleteTask(int? idTask)
        {
            if(!await dbService.TaskService.DeleteTaskAsync(idTask)) return RedirectToAction("EditTask", idTask);

            return RedirectToAction("HomePage", "Home");
        }
    }
}
