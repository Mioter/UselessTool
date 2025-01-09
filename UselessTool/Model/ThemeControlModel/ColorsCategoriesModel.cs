using System.Collections.ObjectModel;

namespace UselessTool.Model.ThemeControlModel;

public class ColorsCategoriesModel(string categoriesName)
{
    public string CategoriesName { get; } = categoriesName;

    public ObservableCollection<ColorsDisplayNameModel> ColorsNameModels { get; set; } = [];
}