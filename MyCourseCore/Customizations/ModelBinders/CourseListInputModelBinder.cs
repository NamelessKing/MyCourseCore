using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using MyCourseCore.Models.InputModels;
using MyCourseCore.Models.Options;
using System.Threading.Tasks;

namespace MyCourseCore.Customizations.ModelBinders
{
    public class CourseListInputModelBinder : IModelBinder
    {
        public IOptionsMonitor<CoursesOptions> CoursesOptions { get; }

        public CourseListInputModelBinder(IOptionsMonitor<CoursesOptions> coursesOptions)
        {
            CoursesOptions = coursesOptions;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //Recuperiamo i valori grazie ai value provider
            string search = bindingContext.ValueProvider.GetValue("Search").FirstValue;
            string orderBy = bindingContext.ValueProvider.GetValue("OrderBy").FirstValue;
            int.TryParse(bindingContext.ValueProvider.GetValue("Page").FirstValue, out int page);
            bool.TryParse(bindingContext.ValueProvider.GetValue("Ascending").FirstValue, out bool ascending);

            //Creiamo l'istanza del CourseListInputModel
            CoursesOptions options = CoursesOptions.CurrentValue;
            var inputModel = new CourseListInputModel(search, orderBy,ascending,page, options.PerPage, options.Order);

            //Impostiamo il risultato per notificare che la creazione è avvenuta con successo
            bindingContext.Result = ModelBindingResult.Success(inputModel);

            //Restituiamo un task completato
            return Task.CompletedTask;
        }
    }
}
