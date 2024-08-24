using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeBattleArena.Helpers
{
    public class MessageTagHelper : TagHelper
    {
        public string UserLogin { get; set; }
        public string Message { get; set; }
        public string FontColor { get; set; }
        public DateTime DateTime { get; set; }
        public int? IdMessage { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (IdMessage == null) { return; }

            output.TagName = "li"; // Используем тег li для создания элемента списка
            output.Attributes.SetAttribute("class", "list-group-item d-flex justify-content-start align-items-center mb-2");

            var span = new TagBuilder("span");
            span.AddCssClass($"alert alert-{FontColor} d-inline-block mb-4 me-1");

            var strong = new TagBuilder("strong");
            strong.InnerHtml.Append(UserLogin + ": ");

            var messageSpan = new TagBuilder("span");
            messageSpan.InnerHtml.Append(Message);

            // Форматирование даты и времени
            var timeSpan = new TagBuilder("span");
            timeSpan.AddCssClass("text-muted ms-2");
            timeSpan.InnerHtml.Append(DateTime.ToString("dd MMMM, HH:mm"));

            span.InnerHtml.AppendHtml(strong);
            span.InnerHtml.AppendHtml(messageSpan);
            span.InnerHtml.AppendHtml(timeSpan);

            output.Content.AppendHtml(span);
        }
    }
    public class AvatarTagHelper : TagHelper
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AvatarTagHelper(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string FolderPath { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string avatarPath = Path.Combine(_webHostEnvironment.WebRootPath, FolderPath);
            List<string> avatarFiles = new List<string>();

            if (Directory.Exists(avatarPath))
            {
                foreach (var file in Directory.GetFiles(avatarPath))
                {
                    avatarFiles.Add(Path.GetFileName(file));
                }
            }

            // Оберните выход в прокручиваемый контейнер
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "avatar-container");
            output.Attributes.SetAttribute("style", "overflow-x: auto; max-height: 200px;");

            var innerDiv = new TagBuilder("div");
            innerDiv.AddCssClass("row flex-nowrap");

            foreach (var avatar in avatarFiles)
            {
                var button = new TagBuilder("button");
                button.Attributes["type"] = "button";
                button.Attributes["onclick"] = $"selectAvatar('/{FolderPath}/{avatar}')";
                button.AddCssClass("btn btn-light col-md-3 m-1");

                var imgTag = new TagBuilder("img");
                imgTag.Attributes["src"] = $"/{FolderPath}/{avatar}";
                imgTag.AddCssClass("img-thumbnail");

                button.InnerHtml.AppendHtml(imgTag);
                innerDiv.InnerHtml.AppendHtml(button);
            }

            output.Content.AppendHtml(innerDiv);
        }
    }
}